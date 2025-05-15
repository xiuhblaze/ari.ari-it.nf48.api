using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Tools
{
    internal class CorsMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken)
                .ContinueWith(task => 
                { 
                    HttpResponseMessage response = task.Result;

                    // Agrega el encabezado CORS a TODAS las respuestas
                    response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:5173");

                    // Solo para solicitudes OPTIONS, agrega los demás encabezados
                    if (request.Method == HttpMethod.Options)
                    {
                        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization, Accept");
                        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                        //response.Headers.Add("Access-Control-Max-Age", "1728000");
                    }

                    return response;
                });
        }
    }
}