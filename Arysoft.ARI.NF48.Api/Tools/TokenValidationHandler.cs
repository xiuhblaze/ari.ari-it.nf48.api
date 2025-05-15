using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using System.Text;

namespace Arysoft.ARI.NF48.Api.Tools
{
    internal class TokenValidationHandler : DelegatingHandler
    {
        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            token = null;
            IEnumerable<string> authzHeaders;
            if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
            {
                return false;
            }
            var bearerToken = authzHeaders.ElementAt(0);
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            return true;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Para solicitudes OPTIONS, responde inmediatamente con cabeceras CORS
            if (request.Method == HttpMethod.Options)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:5173");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization, Accept");
                response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                return Task.FromResult(response);
            }

            //HttpStatusCode statusCode;

            // Validar el token JWT
            string token;
            if (!TryRetrieveToken(request, out token))
            {
                // No hay token, pero permitimos que continúe el procesamiento para que
                // los controladores puedan decidir si la solicitud debe ser autorizada
                // statusCode = HttpStatusCode.Unauthorized;
                return base.SendAsync(request, cancellationToken);
            }

            try
            {
                var secretKey = ConfigurationManager.AppSettings["JwtKey"];
                var audienceToken = ConfigurationManager.AppSettings["JwtAudience"];
                var issuerToken = ConfigurationManager.AppSettings["JwtIssuer"];
                var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(secretKey));

                SecurityToken securityToken;
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                TokenValidationParameters validationParameters = new TokenValidationParameters()
                {
                    ValidAudience = audienceToken,
                    ValidIssuer = issuerToken,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    LifetimeValidator = this.LifetimeValidator,
                    IssuerSigningKey = securityKey
                };

                // Extract and assign Current Principal and user
                Thread.CurrentPrincipal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
                HttpContext.Current.User = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return base.SendAsync(request, cancellationToken)
                    .ContinueWith(task => {
                        var response = task.Result;

                        // Importante: Añadir cabecera CORS a TODAS las respuestas
                        if (!response.Headers.Contains("Access-Control-Allow-Origin"))
                        {
                            response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:5173");
                        }

                        return response;
                    }, cancellationToken);
            }
            catch (SecurityTokenValidationException)
            {
                //statusCode = HttpStatusCode.Unauthorized;
                return CreateCorsEnabledResponse(HttpStatusCode.Unauthorized);
            }
            catch (Exception)
            {
                //statusCode = HttpStatusCode.InternalServerError;
                return CreateCorsEnabledResponse(HttpStatusCode.InternalServerError);
            }

            //return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode) { });
        } // SendAsync

        private Task<HttpResponseMessage> CreateCorsEnabledResponse(HttpStatusCode statusCode)
        {
            var response = new HttpResponseMessage(statusCode);

            // Agregar encabezados CORS a la respuesta de error
            response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:5173");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization, Accept");
            response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");

            return Task.FromResult(response);
        }

        private bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters parameters)
        {
            // Validate the token expiration time
            if (expires != null)
            {
                return expires > DateTime.UtcNow;
            }

            return false;
        }
    }
}