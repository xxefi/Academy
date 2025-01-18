using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Academy.Presentation.Middlewares;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly ConcurrentDictionary<string, Queue<DateTime>> RequestLog = new();
    private static readonly TimeSpan BlockDuration = TimeSpan.FromSeconds(60); 
    private static readonly int RequestLimit = 10; 
    
    public RateLimitingMiddleware(RequestDelegate next)
        => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        if (ipAddress == null)
        {
            await _next(context);
            return;
        }
        var requestQueue = RequestLog.GetOrAdd(ipAddress, new Queue<DateTime>());

        lock (requestQueue)
        {
            while (requestQueue.Count > 0 && requestQueue.Peek() < DateTime.UtcNow - BlockDuration)
                requestQueue.Dequeue();
        
            if (requestQueue.Count >= RequestLimit)
            {
                var timeUntilUnlock = BlockDuration - (DateTime.UtcNow - requestQueue.Peek());
                
                context.Response.StatusCode = (int)StatusCodes.Status429TooManyRequests;
                context.Response.ContentType = "application/json";
                
                var responseObject = new
                {
                    Code = (int)StatusCodes.Status429TooManyRequests,
                    Message = "1 DEQIQELIK BLOKLANDIN",
                    BlockDuration = timeUntilUnlock.TotalSeconds
                };
                
                var errorResponse = JsonSerializer.Serialize(responseObject, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

                context.Response.WriteAsync(errorResponse);
                return;
            }
            requestQueue.Enqueue(DateTime.UtcNow);
        }
        
        await _next(context);
    }
}