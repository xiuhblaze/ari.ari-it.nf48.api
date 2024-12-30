using Microsoft.Owin.Security.OAuth;
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
            //var cors = new EnableCorsAttribute("http://localhost:5173,http://localhost:3000", "*", "*");            
            var corsDev = new EnableCorsAttribute("http://localhost:5173", "*", "*");
            //var corsProduction = new EnableCorsAttribute("https://aarrin.com,http://aarrin.com,http://localhost:5173,http://cortana.im-prove.com.mx", "*", "*");

            config.EnableCors(corsDev);
            //config.EnableCors(corsProduction);

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
