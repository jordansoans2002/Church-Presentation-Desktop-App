namespace control_ppt_server.Models.Responses
{
    public class OperationResult
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public PresentationInfo? PresentationInfo { get; set; } 

        public static OperationResult CreateSuccess(int StatusCode = 200, string message = "", PresentationInfo? info = null) =>
            new OperationResult { StatusCode = StatusCode, Message = message, PresentationInfo = info };
        public static OperationResult CreateError(int StatusCode = 400, string message = "", PresentationInfo? info = null) => 
            new OperationResult { StatusCode = StatusCode, Message = message, PresentationInfo = info };
    }
}
