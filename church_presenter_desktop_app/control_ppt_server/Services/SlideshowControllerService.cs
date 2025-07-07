using control_ppt_server.Models;
using control_ppt_server.Models.Responses;
using control_ppt_server.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;

namespace control_ppt_server.Services
{
    public class SlideshowControllerService : ISlideshowControlService
    {
        private Application? App { get; set; } = null;
        public Dictionary<string, Presentation> Presentations { get; set; }

        private const int THUMBNAIL_WIDTH = 240;
        private const int THUMBNAIL_HEIGHT = 180;

        private string GenerateStableId(Presentation presentation)
        {
            try
            {
                // Combine full path and creation date for uniqueness
                string uniqueIdentifier = presentation.FullName;
                // Create a hash of the identifier to keep it manageable
                return Convert.ToBase64String(System.Security.Cryptography.MD5.Create()
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(uniqueIdentifier)))
                    .Replace("/", "_").Replace("+", "-");
            }
            catch
            {
                // Fallback to Guid if we can't access the file properties
                return Guid.NewGuid().ToString();
            }
        }


        /// <summary>
        /// Creates a powerpoint instance and finds all active Presentations
        /// Throws error if there are no PowerPoint instances
        /// Catches error that an instance does not have an active slideshow
        /// </summary>
        /// <returns>List<PresentationInfo></returns>
        public List<PresentationInfo> GetActiveSlideshows()
        {
            CleanupExistingInstances();

            var allPresentations = new List<PresentationInfo>();

            // connects to the powerpoint instance, throws error if there are not active PowerPoint instances
            App = Marshal2.GetActiveObject("PowerPoint.Application") as Application;
            if (App != null)
            {
                Presentations = new Dictionary<string, Presentation>();

                //App.ActivePresentation returns the presentation that was last selected
                //Debug.WriteLine(App.ActivePresentation.FullName);

                foreach (Presentation pres in App.Presentations)
                {
                    try
                    {
                        // pres.SlideshowWindow throws error if the slideshow is not active for an instance
                        var test  = pres.SlideShowWindow;
                        string stableId = GenerateStableId(pres);
                        Presentations[stableId] = pres;
                        allPresentations.Add(new PresentationInfo(stableId, pres));
                    } catch
                    {
                        Debug.WriteLine($"Slideshow is not active for {pres.Name} instance");
                    }
                }
            }

            return allPresentations;            
        }


        /// <summary>
        /// Checks whether the current slide + change slide exists for the presentation
        /// If present returns the slideshow view and the new position
        /// If the slide does not exist returns 0
        /// If the presentation does not exist return -1
        /// </summary>
        /// <param name="presentationId"></param>
        /// <param name="slideChange"></param>
        /// <returns>SlideShowView for presentation id and the new position</returns>
        public (Presentation?, int) DoesSlideExist(string presentationId, int slideChange)
        {
            if (Presentations.TryGetValue(presentationId, out var presentation))
            {
                try
                {
                    var slideshow = presentation.SlideShowWindow.View;
                    int newPosition = slideshow.CurrentShowPosition + slideChange;

                    if (newPosition < 1 || newPosition > presentation.Slides.Count)
                    {
                        return (presentation,0);
                    }
                    else
                    {
                        return (presentation, newPosition);
                    }
                }
                catch (COMException ce)
                {
                    Debug.WriteLine(ce.Message);
                    return (null, -1);
                }
            }

            return (null, -1);
        }

        public void ChangeSlide(Presentation presentation, int newPosition, SlideshowPreviewOptions options)
        {
            presentation.SlideShowWindow.View.GotoSlide(newPosition, Microsoft.Office.Core.MsoTriState.msoTrue);
                 
        }


        public async Task<(byte[] imageData, string contentType)?> GetSlidePreview(string presentationId, int slideNumber)
        {
           
                if (Presentations.TryGetValue(presentationId, out var presentation))
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
            

            return null;
        }


        private void CleanupExistingInstances()
        {
            try
            {
                if (Presentations != null)
                {
                    foreach (var pres in Presentations.Values)
                    {
                        if (pres != null)
                            Marshal.ReleaseComObject(pres);
                    }
                }
                if (App != null)
                    Marshal.ReleaseComObject(App);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error cleaning up COM objects: {ex.Message}");
            }            
        }


        public void Dispose()
        {
            CleanupExistingInstances();
            
        }
    }
}