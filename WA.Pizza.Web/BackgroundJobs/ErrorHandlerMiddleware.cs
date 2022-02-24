using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using WA.Pizza.Core.Exceptions;
using WA.Pizza.Web.Extensions;

namespace WA.Pizza.Web.BackgroundJobs
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ItemNotFoundException infEx)
            {
                await HandleExceptionAsync(httpContext, infEx);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var pathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            context.Response.ContentType = "application/json";
            if (exception.GetType().FullName == "ItemNotFoundException")
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
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
