using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using System.Security.Authentication;
using WA.Pizza.Core.Exceptions;
using WA.Pizza.Infrastructure.Data;
using WA.Pizza.Infrastructure.Data.Seed;
using WA.Pizza.Web.BackgroundJobs;

namespace WA.Pizza.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void SeedDatabase(this IApplicationBuilder applicationBuilder)
        {
            using var services = applicationBuilder.ApplicationServices.CreateScope();

            var dbContext = services.ServiceProvider.GetService<AppDbContext>();
            new CatalogItemSeeder().SeedAsync(dbContext).GetAwaiter().GetResult();
        }

        public static void ExceptionHandlerConfigure(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Run(async context =>
            {
                context.Response.ContentType = "application/json";
                IExceptionHandlerPathFeature? pathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                Exception? ex = pathFeature?.Error;

                if (ex == null)
                    return;

                context.Response.StatusCode = ex switch
                {
                    ItemNotFoundException => StatusCodes.Status404NotFound,
                    InvalidCredentialException or AuthenticationException => 441,
                    _ => StatusCodes.Status500InternalServerError
                };

                object response = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Path = pathFeature?.Path,
                    InnerExceptionMessages = ex.GetInnerExceptionMessages()
                };

                Log.Error(ex.Message, response);

                if (!env.IsDevelopment())
                    response = new { ex.Message };

                await context.Response.WriteAsJsonAsync(response);
            });

        }
    }
}
