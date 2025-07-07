namespace control_ppt_server.Models.Requests
{
    public class SlideChangeRequest
    {
        public required List<string> PresentationIds { get; set; }
        public int SlideChange { get; set; }
        public SlideshowPreviewOptions Options { get; set; } = new SlideshowPreviewOptions();
    }
}
