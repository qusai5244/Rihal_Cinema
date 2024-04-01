namespace Rihal_Cinema.Helpers
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ApiResponse(bool status, int statusCode, string message, T data)
        {
            Success = status;
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }
    }
}
