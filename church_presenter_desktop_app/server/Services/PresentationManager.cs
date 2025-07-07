namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Microsoft.Office.Core;
    using Microsoft.Office.Interop.PowerPoint;
    using Models;
    using Models.Responses;

    public class PresentationManager : IPresentationManager
    {
        private class PowerPointInstance
        {
            public Application App { get; set; }
            public Dictionary<string, Presentation> Presentations { get; set; }
        }

        private Dictionary<string, PowerPointInstance> powerPointInstances;

        private const int THUMBNAIL_WIDTH = 240;
        private const int THUMBNAIL_HEIGHT = 180;

        public PresentationManager()
        {
            powerPointInstances = new Dictionary<string, PowerPointInstance>();
            RefreshAllInstances();
        }

        private void RefreshAllInstances()
        {
            // Clear existing instances
            foreach (var instance in powerPointInstances.Values)
            {
                try
                {
                    if (instance.Presentations != null)
                    {
                        foreach (var pres in instance.Presentations.Values)
                        {
                            if (pres != null)
                                Marshal.ReleaseComObject(pres);
                        }
                    }
                    if (instance.App != null)
                        Marshal.ReleaseComObject(instance.App);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error cleaning up COM objects: {ex.Message}");
                }
            }
            powerPointInstances.Clear();

            try
            {
                // Find all PowerPoint processes
                var pptProcesses = Process.GetProcessesByName("POWERPNT");

                foreach (var process in pptProcesses)
                {
                    try
                    {
                        var app = Marshal2.GetActiveObject("PowerPoint.Application") as Application;
                        if (app == null) continue;

                        var presentations = new Dictionary<string, Presentation>();

                        if (app.Presentations != null)
                        {
                            foreach (Presentation pres in app.Presentations)
                            {
                                if (pres != null)
                                {
                                    string id = Guid.NewGuid().ToString();
                                    presentations[id] = pres;
                                }
                            }
                        }

                        powerPointInstances[process.Id.ToString()] = new PowerPointInstance
                        {
                            App = app,
                            Presentations = presentations
                        };
                    }
                    catch (COMException ex)
                    {
                        Debug.WriteLine($"Error accessing PowerPoint instance: {ex.Message}");
                        continue;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Unexpected error: {ex.Message}");
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in RefreshAllInstances: {ex.Message}");
                throw;
            }
        }

        public List<PresentationInfo> GetActivePresentations()
        {
            RefreshAllInstances();

            var allPresentations = new List<PresentationInfo>();

            foreach (var instance in powerPointInstances)
            {
                foreach (var pres in instance.Value.Presentations)
                {
                    //if a ppt is active and has no slide show this line throws an error
                    allPresentations.Add(new PresentationInfo
                    {
                        Id = pres.Key,
                        Name = System.IO.Path.GetFileNameWithoutExtension(pres.Value.FullName),
                        CurrentSlide = pres.Value.SlideShowWindow?.View?.CurrentShowPosition ?? 1,
                        TotalSlides = pres.Value.Slides.Count,
                        WindowTitle = pres.Value.Windows.Count > 0 ? pres.Value.Windows[1].Caption : "Unknown",
                        IsRunning = pres.Value.SlideShowWindow != null,
                        ProcessId = instance.Key
                    });
                }
            }

            return allPresentations;
        }

        public OperationResult ChangeSlide(string presentationId, int slideChange)
        {
            foreach (var instance in powerPointInstances)
            {
                if (instance.Value.Presentations.TryGetValue(presentationId, out var presentation))
                {
                    var slideShow = presentation.SlideShowWindow?.View;
                    if (slideShow == null)
                    {
                        return OperationResult.CreateError("Presentation is not in slide show mode");
                    }

                    int newPosition = slideShow.CurrentShowPosition + slideChange;
                    
                    if (newPosition < 1 || newPosition > presentation.Slides.Count)
                    {
                        return OperationResult.CreateError($"Invalid slide number. Must be between 1 and {presentation.Slides.Count}");
                    }

                    slideShow.GotoSlide(newPosition,Microsoft.Office.Core.MsoTriState.msoTrue);
                    return OperationResult.CreateSuccess("Slide changed successfully");
                }
            }
            
            return OperationResult.CreateError("Presentation not found");
        }

        public OperationResult ControlSlideShow(string presentationId, bool start)
        {
            foreach (var instance in powerPointInstances)
            {
                if (instance.Value.Presentations.TryGetValue(presentationId, out var presentation))
                {
                    try
                    {
                        if (start)
                        {
                            if (presentation.SlideShowWindow == null)
                            {
                                presentation.SlideShowSettings.Run();
                                return OperationResult.CreateSuccess("Slide show started");
                            }
                            return OperationResult.CreateError("Slide show is already running");
                        }
                        else
                        {
                            if (presentation.SlideShowWindow != null)
                            {
                                presentation.SlideShowWindow.View.Exit();
                                return OperationResult.CreateSuccess("Slide show stopped");
                            }
                            return OperationResult.CreateError("Slide show is not running");
                        }
                    }
                    catch (COMException ex)
                    {
                        return OperationResult.CreateError($"Error controlling slide show: {ex.Message}");
                    }
                }
            }

            return OperationResult.CreateError("Presentation not found");
        }

        public async Task<(byte[] imageData, string contentType)?> GetSlidePreview(string presentationId, int slideNumber)
        {
            foreach (var instance in powerPointInstances)
            {
                if (instance.Value.Presentations.TryGetValue(presentationId, out var presentation))
                {
                    try
                    {
                        if (slideNumber < 1 || slideNumber > presentation.Slides.Count)
                        {
                            return null;
                        }

                        var slide = presentation.Slides[slideNumber];
                        string tempPath = Path.GetTempFileName();

                        // Export slide as PNG
                        slide.Export(tempPath, "PNG", THUMBNAIL_WIDTH, THUMBNAIL_HEIGHT);

                        // Read the file and return its contents
                        var imageBytes = await File.ReadAllBytesAsync(tempPath);
                        File.Delete(tempPath);

                        return (imageBytes, "image/png");
                    }
                    catch (COMException)
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        public void Dispose()
        {
            foreach (var instance in powerPointInstances)
            {
                foreach (var pres in instance.Value.Presentations.Values)
                {
                    Marshal.ReleaseComObject(pres);
                }
                //Marshal.ReleaseComObject(instance.app as object); //error
            }
        }
    }
}