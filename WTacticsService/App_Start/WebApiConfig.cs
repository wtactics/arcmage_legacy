using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WTacticsService.Api.Authentication;
using WTacticsService.Helpers;

namespace WTacticsService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Filters.Add(new AuthenticationAttribute()); // Every call should be authenticated
            config.Filters.Add(new WebApiExceptionFilterAttribute()); // Every exception should be logged

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
