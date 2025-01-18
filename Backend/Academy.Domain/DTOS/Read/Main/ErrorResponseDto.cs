namespace Academy.Domain.DTOS.Read.Main;

public class ErrorResponseDto
{
    public class ErrorResponse
    {
        public object Data { get; set; }
        public bool Success { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public string Ex { get; set; }
        public DateTime RequestDate { get; set; }
        public long Ticks { get; set; }
        public string RequestId { get; set; }
        public string? ActivityTraceId { get; set; }
        public ErrorResponseDetails Details { get; set; }
        public ClientInfoDto ClientInfo { get; set; }
    }

    public class ErrorResponseDetails
    {
        public string Path { get; set; }
        public string HttpMethod { get; set; }
    }
}