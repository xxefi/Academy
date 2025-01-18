namespace Academy.Domain.Entities;

public class RequestLogEntity
{
    public int Id { get; set; }
    public string RequestId { get; set; } 
    public string ClientIP { get; set; }
    public string UserAgent { get; set; }
    public string Path { get; set; }
    public string Method { get; set; } 
    public DateTime RequestDate { get; set; }
    public long Ticks { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } 
    public string? ExceptionType { get; set; }
    public string? ErrorLocation { get; set; }
}