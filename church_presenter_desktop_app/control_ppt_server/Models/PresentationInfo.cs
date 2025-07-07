using Microsoft.Office.Interop.PowerPoint;

namespace control_ppt_server.Models
{
    public class PresentationInfo
    {
        public string PresentationId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string WindowTitle { get; set; } = string.Empty;
        public bool IsRunning { get; set; }
        public int CurrentSlide {  get; set; }
        public int TotalSlides { get; set; }

        public PresentationInfo(string presentationId, Presentation presentation) {
            PresentationId = presentationId;
            FileName = System.IO.Path.GetFileNameWithoutExtension(presentation.FullName);
            FilePath = presentation.FullName;
            CurrentSlide = presentation.SlideShowWindow?.View?.CurrentShowPosition ?? 1;
            TotalSlides = presentation.Slides.Count;
            WindowTitle = presentation.Windows.Count > 0 ? presentation.Windows[1].Caption : "Unknown";
            IsRunning = presentation.SlideShowWindow != null;

        }

    }
}
