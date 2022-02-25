using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text;
using WA.Pizza.Core.Exceptions;
using WA.Pizza.Web.Extensions;

namespace WA.Pizza.Web.BackgroundJobs
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ItemNotFoundException infEx)
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                await HandleExceptionAsync(httpContext, infEx);
            }
            catch (Exception ex)
            {
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var pathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            _logger.LogError($"At path: {pathFeature?.Path} occured {exception.GetType().Name} with status code: {context.Response.StatusCode} and message: {exception.Message}. Stack trace: {exception.StackTrace}. Inner exceptions: {string.Join(Environment.NewLine, exception.GetInnerExceptionMessages())}");
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                StatusCode = context.Response.StatusCode,
                StackTrace = exception.StackTrace,
                Message = exception.Message,
                Path = pathFeature?.Path,
                InnerExceptionMessages = exception.GetInnerExceptionMessages()
            }.ToString());

        }
    }
}
