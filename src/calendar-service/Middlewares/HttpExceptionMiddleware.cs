using System;
using System.Net;
using System.Threading.Tasks;
using Calendar.Service.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Calendar.Service.Middlewares;

internal class HttpExceptionMiddleware
{
    private readonly ILogger<HttpExceptionMiddleware> logger;
    private readonly RequestDelegate next;

    public HttpExceptionMiddleware(RequestDelegate next, ILogger<HttpExceptionMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = ex switch
            {
                HttpException =>
                    (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            await response.WriteAsync(ex.Message);

            var request = context.Request.HttpContext.Request;
            const string errorMessage = "when {Method} request {Path}, Exception occurs : {Message}";
            logger.LogError(ex, errorMessage, request.Method, request.Path, ex.Message);
        }
    }
}

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseHttpException(this IApplicationBuilder application)
    {
        return application.UseMiddleware<HttpExceptionMiddleware>();
    }
}