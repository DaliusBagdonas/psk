﻿using System.Web.Mvc;
using System.Web.Routing;

namespace PSK.WebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.LowercaseUrls = true;
            routes.MapMvcAttributeRoutes();

            AreaRegistration.RegisterAllAreas();

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "PSK.Web.Controllers" }
            );

            routes.MapRoute(
                "LogOff",
                "Login/LogOff",
                new { controller = "Login", action = "LogOff"}
            );
        }
    }
}