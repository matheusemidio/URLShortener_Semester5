using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FinalExam_1931358
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Redirect default",
                url: "u/{shortUrlParameter}",
                defaults: new { controller = "urls", action = "Go", url = UrlParameter.Optional }
            );
            //If the user types a long url, it should be created with an auto-generated short_url
            //Not currently working
            routes.MapRoute(
                name: "Redirect with long url",
                url: "v/{longUrlParameter}",
                defaults: new { controller = "urls", action = "Generator", url = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
