using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Arysoft.ARI.NF48.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Configuración y servicios de Web API

            // CORS
            var cors = new EnableCorsAttribute("http://localhost:5173,http://localhost:3000", "*", "*");
            //var cors = new EnableCorsAttribute("http://ari.arysoft.com.mx", "*", "*");
            //var corsProduccion = new EnableCorsAttribute("https://aarrin.com", "*", "*");

            config.EnableCors(cors);

            // Rutas de Web API
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
