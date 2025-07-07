namespace control_ppt_server.Models.Responses
{
    public class SlideChangeResponse
    {
        // TODO response should be list of dict
        // response can also be success if all slide changes succeed or fail if any one fails
        public bool Success { get; set; } = true;
        public List<object> Presentations { get; set; } = new List<object>();
    }
}
