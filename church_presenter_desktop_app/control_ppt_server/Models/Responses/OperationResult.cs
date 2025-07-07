namespace control_ppt_server.Models.Responses
{
    public class OperationResult
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public static OperationResult CreateSuccess(int StatusCode = 200, string message = "") =>
            new OperationResult { StatusCode = StatusCode, Message = message };
        public static OperationResult CreateError(int StatusCode = 400, string message = "") => 
            new OperationResult { StatusCode = StatusCode, Message = message };
    }
}
