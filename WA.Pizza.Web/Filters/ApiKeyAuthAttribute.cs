using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WA.Pizza.Web.Filters
{
	[AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
	public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
	{
		private const string API_KEY_AUTH_ATTRIBUTE_NAME = "ApiKey";

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			if (!context.HttpContext.Request.Headers.TryGetValue(API_KEY_AUTH_ATTRIBUTE_NAME, out var potentialApiKey))
			{
				context.Result = new UnauthorizedResult();
				return;
			}

			var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
			var apiKey = configuration.GetValue<string>(API_KEY_AUTH_ATTRIBUTE_NAME);

			if (!apiKey.Equals(potentialApiKey))
			{
				context.Result = new UnauthorizedResult();
				return;
			}

			await next();
		}
	}
}
