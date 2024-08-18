using Academy.Server.Services.Interfaces;

namespace ApiFirst.Middlewares;

public class JwtSessionMiddleware : IMiddleware
{
    private readonly IBlackListService _blackListService;

    public JwtSessionMiddleware(IBlackListService blackListService)
    {
        _blackListService = blackListService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string token = context.Request.Headers["Authorization"];
        token = token?.Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
        {
            await next(context);
            return;
        }

        if (_blackListService.IsTokenBlackListed(token))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }
        await next(context);
    }
}
