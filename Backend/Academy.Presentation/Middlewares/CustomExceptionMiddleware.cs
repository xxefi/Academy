using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Academy.Application.Exceptions;
using Academy.Domain.Abstractions.Services.Main;
using Academy.Domain.DTOS.Read.Main;
using Academy.Domain.Entities;
using Academy.Infrastructure.Context;
using NanoidDotNet;

namespace Academy.Presentation.Middlewares;

public class CustomExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILocalizationService _localizationService;

    public CustomExceptionMiddleware(RequestDelegate next, IServiceProvider serviceProvider, ILocalizationService localizationService)
    {
        _next = next;
        _serviceProvider = serviceProvider;
        _localizationService = localizationService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AcademyException ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, AcademyException exception)
    {
        var requestId = Nanoid.Generate(size: 64);
        var language = GetLanguageFromRequest(context);
        var statusCode = (int)HttpStatusCode.InternalServerError;
        var errorResponse = CreateErrorResponse(context, exception, requestId, language, statusCode);
        
        if (exception is AcademyException academyExceptions)
        {
            statusCode = GetStatusCodeForExceptionType(academyExceptions.ExceptionType);
            errorResponse.Code = statusCode;
            errorResponse.Message = _localizationService.GetMessage(academyExceptions.Message, language);
            errorResponse.Ex = academyExceptions.ExceptionType.ToString();
        }
        
        await LogRequestAsync(context, exception, requestId, statusCode);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        
        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
        
        await context.Response.WriteAsync(jsonResponse);
    }
    
    private ErrorResponseDto.ErrorResponse CreateErrorResponse(HttpContext context, AcademyException exception, string requestId, string language, int statusCode)
    {
        return new ErrorResponseDto.ErrorResponse
        {
            Success = false,
            Code = statusCode,
            Message = _localizationService.GetMessage(exception.Message, language),
            Ex = exception.ExceptionType.ToString(),
            RequestDate = DateTime.UtcNow,
            Ticks = DateTime.UtcNow.Ticks,
            RequestId = requestId,
            ActivityTraceId = context.TraceIdentifier,
            Details = new ErrorResponseDto.ErrorResponseDetails
            {
                Path = context.Request.Path,
                HttpMethod = context.Request.Method,
            },
            ClientInfo = new ClientInfoDto
            {
                UserAgent = context.Request.Headers["User-Agent"].FirstOrDefault(),
                IpAddress = GetClientIpAddress(context),
            }
        };
    }
    
    private async Task LogRequestAsync(HttpContext context, AcademyException exception, string requestId, int statusCode)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AcademyContext>();
        var errorLocation = $"{exception.StackTrace}";

        var logEntry = new RequestLogEntity
        {
            RequestId = requestId,
            ClientIP = GetClientIpAddress(context),
            UserAgent = context.Request.Headers["User-Agent"].FirstOrDefault(),
            Path = context.Request.Path,
            Method = context.Request.Method,
            RequestDate = DateTime.UtcNow,
            Ticks = DateTime.UtcNow.Ticks,
            StatusCode = statusCode,
            Message = exception.Message,
            ExceptionType = exception.ExceptionType.ToString(),
            ErrorLocation = errorLocation,
        };
        
        await dbContext.RequestLogs.AddAsync(logEntry);
        await dbContext.SaveChangesAsync();
    }

    private int GetStatusCodeForExceptionType(ExceptionType exceptionType)
    {
        return exceptionType switch
        {
            ExceptionType.InvalidToken => (int)HttpStatusCode.BadRequest,
            ExceptionType.InvalidRefreshToken => (int)HttpStatusCode.BadRequest,
            ExceptionType.InvalidCredentials => (int)HttpStatusCode.BadRequest,
            ExceptionType.UserNotFound => (int)HttpStatusCode.NotFound,
            ExceptionType.NullCredentials => (int)HttpStatusCode.BadRequest,
            ExceptionType.InvalidRequest => (int)HttpStatusCode.BadRequest,
            ExceptionType.PasswordMismatch => (int)HttpStatusCode.BadRequest,
            ExceptionType.EmailAlreadyConfirmed => (int)HttpStatusCode.BadRequest,
            ExceptionType.EmailNotConfirmed => (int)HttpStatusCode.BadRequest,
            ExceptionType.EmailAlreadyExists => (int)HttpStatusCode.BadRequest,
            ExceptionType.CredentialsAlreadyExists => (int)HttpStatusCode.BadRequest,
            ExceptionType.NotFound => (int)HttpStatusCode.NotFound,
            ExceptionType.UnauthorizedAccess => (int)HttpStatusCode.Unauthorized,
            ExceptionType.Forbidden => (int)HttpStatusCode.Forbidden,
            ExceptionType.BadRequest => (int)HttpStatusCode.BadRequest,
            ExceptionType.Conflict => (int)HttpStatusCode.Conflict,
            ExceptionType.InternalServerError => (int)HttpStatusCode.InternalServerError,
            ExceptionType.ServiceUnavailable => (int)HttpStatusCode.ServiceUnavailable,
            ExceptionType.OperationFailed => (int)HttpStatusCode.BadRequest,
            ExceptionType.DatabaseError => (int)HttpStatusCode.InternalServerError,
            ExceptionType.Critical => (int)HttpStatusCode.InternalServerError,
            _ => (int)HttpStatusCode.InternalServerError,
        };
    }

    private string GetLanguageFromRequest(HttpContext context)
    {
        string defaultLang = "en";
        var queryLang = context.Request.Query["lang"].ToString().ToLower();
        var headerLang = context.Request.Headers["Accept-Language"].ToString().Split(',')[0].ToLower();

        if (!string.IsNullOrEmpty(queryLang) && (queryLang == "ru" || queryLang == "az" || queryLang == "en"))
            return queryLang;

        if (!string.IsNullOrEmpty(headerLang) && (headerLang == "ru" || headerLang == "az" || headerLang == "en"))
            return headerLang;

        return defaultLang;
    }
    
    private string GetClientIpAddress(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        return !string.IsNullOrEmpty(forwardedFor) ? forwardedFor.Split(',').FirstOrDefault() : context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
    
}
