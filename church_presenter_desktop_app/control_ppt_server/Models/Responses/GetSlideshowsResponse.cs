namespace control_ppt_server.Models.Responses
{
    public class GetSlideshowsResponse
    {
        public List<PresentationInfo>? presentations { get; set; } = null;

        //can optionally add a list of powerpoint instances that are not in slideshow mode
    }
}
