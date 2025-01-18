using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Academy.Domain.DTOS.Read.Main;
using Academy.Domain.Entities;
using Academy.Infrastructure.Context;
using NanoidDotNet;

namespace Academy.Presentation.Middlewares;

public class CustomSuccessResponseMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public CustomSuccessResponseMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }
        var requestId = Nanoid.Generate(size: 64);
        var originalBodyStream = context.Response.Body;
        
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AcademyContext>();
        try
        {
            await _next(context);
        
            responseBody.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(responseBody).ReadToEndAsync();
        
            var clientInfo = GetClientInfo(context);
        
            var modifiedResponse = await BuildResponseAsync(context, responseText, requestId, clientInfo);
        
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                await HandleSuccessResponse(context, originalBodyStream, responseBody, modifiedResponse, requestId, dbContext, clientInfo);
            else
                await HandleErrorResponse(originalBodyStream, responseBody);
        }
        catch (Exception ex)
        {
            var responseObject = new ResponseDto<object>
            {
                Success = false,
                Code = 500,
                Message = ex.Message,
                RequestDate = DateTime.UtcNow,
                Ticks = DateTime.UtcNow.Ticks,
                RequestId = requestId,
                ClientInfo = GetClientInfo(context)
            };
            
            var logEntry = new RequestLogEntity
            {
                RequestId = requestId,
                ClientIP = GetClientIpAddress(context),
                UserAgent = context.Request.Headers["User-Agent"].FirstOrDefault(),
                Path = context.Request.Path,
                Method = context.Request.Method,
                RequestDate = DateTime.UtcNow,
                Ticks = DateTime.UtcNow.Ticks,
                StatusCode = responseObject.Code,
                Message = ex.Message,
                ErrorLocation = ex.StackTrace,
            };
        
            await dbContext.RequestLogs.AddAsync(logEntry);
            await dbContext.SaveChangesAsync();

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var errorResponse = JsonSerializer.Serialize(responseObject, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            await originalBodyStream.WriteAsync(Encoding.UTF8.GetBytes(errorResponse));
        }
    }

    private static async Task HandleSuccessResponse(
        HttpContext context,
        Stream originalBodyStream,
        Stream responseBody,
        string modifiedResponse,
        string requestId,
        AcademyContext dbContext, 
        ClientInfoDto clientInfo)
    {
        if (context.Response.ContentType != null && context.Response.ContentType.Contains("application/json"))
        {
            context.Response.ContentType = "application/json";
            await originalBodyStream.WriteAsync(Encoding.UTF8.GetBytes(modifiedResponse));
        }
        else
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
        
        var requestLog = new RequestLogEntity
        {
            RequestId = requestId,
            ClientIP = clientInfo.IpAddress,
            UserAgent = clientInfo.UserAgent,
            Path = context.Request.Path,
            Method = context.Request.Method,
            RequestDate = DateTime.UtcNow,
            Ticks = DateTime.UtcNow.Ticks,
            StatusCode = context.Response.StatusCode,
            Message = "Success"
        };
        
        await dbContext.RequestLogs.AddAsync(requestLog);
        await dbContext.SaveChangesAsync();
    }
    private static async Task HandleErrorResponse(Stream originalBodyStream, Stream responseBody)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
    }
    private static async Task<string> BuildResponseAsync(HttpContext context, string responseText, string requestId, ClientInfoDto clientInfo)
    {
        if (string.IsNullOrWhiteSpace(responseText)) responseText = "{}";
        
        var data = await JsonSerializer.DeserializeAsync<object>(
            new MemoryStream(Encoding.UTF8.GetBytes(responseText)));
        
        var responseObject = new ResponseDto<object>
        {
            Data = data,
            Success = true,
            Code = context.Response.StatusCode,
            RequestDate = DateTime.UtcNow,
            Ticks = DateTime.UtcNow.Ticks,
            RequestId = requestId,
            ClientInfo = clientInfo,
        };

        return JsonSerializer.Serialize(responseObject, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
    }
    private static ClientInfoDto GetClientInfo(HttpContext context)
    {
        return new ClientInfoDto
        {
            UserAgent = context.Request.Headers["User-Agent"].FirstOrDefault(),
            IpAddress = GetClientIpAddress(context)
        };
    }
    
    private static string GetClientIpAddress(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
            return forwardedFor.Split(',').FirstOrDefault();

        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}