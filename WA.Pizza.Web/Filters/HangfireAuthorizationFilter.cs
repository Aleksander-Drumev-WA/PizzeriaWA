﻿using Hangfire.Dashboard;

namespace WA.Pizza.Web.Filters
{
	public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            return !httpContext.User.Identity.IsAuthenticated;
        }
	
	}
}
