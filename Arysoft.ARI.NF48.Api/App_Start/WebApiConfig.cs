using Microsoft.Owin.Security.OAuth;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Arysoft.ARI.NF48.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            // Configuración y servicios de Web API
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // CORS - Desarrollo
            //var cors = new EnableCorsAttribute("http://localhost:5173,http://localhost:84", "*", "*");            
            // CORS - Producción
            var cors = new EnableCorsAttribute("https://aarrin.com,http://aarrin.com,https://ariit.aarrin.com,http://ariit.aarrin.com", "*", "*");
            
            config.EnableCors(cors);

            // Rutas de Web API
            config.MapHttpAttributeRoutes();
            //config.MessageHandlers.Add(new Tools.CorsMessageHandler());  // Primero el handler CORS
            //config.MessageHandlers.Add(new TokenValidationHandler());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
