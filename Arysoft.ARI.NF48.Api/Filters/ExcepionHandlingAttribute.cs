using Arysoft.ARI.NF48.Api.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Hosting;
using System.Web.Http.Filters;

namespace Arysoft.ARI.NF48.Api.Filters
{
    public class ExcepionHandlingAttribute : ExceptionFilterAttribute
    {
        private const string GenericServerMessage =
            "An unexpected error occurred. Please try again or contact the administrator.";

        private static readonly object _logLock = new object();

        /// <summary>
        /// Web.config -> appSettings -> ExposeErrorDetails = true | false.
        /// Si falta o no es "true", NO se expone el detalle al cliente (seguro por defecto).
        /// </summary>
        private static bool ExposeErrorDetails =>
            bool.TryParse(ConfigurationManager.AppSettings["ExposeErrorDetails"], out var value) && value;

        public override void OnException(HttpActionExecutedContext context)
        {
            var exception = context.Exception;

            // Status: el definido en BusinessException, 400 por defecto; 500 para todo lo demás
            HttpStatusCode statusCode;
            if (exception is BusinessException businessException)
                statusCode = businessException.StatusCode ?? HttpStatusCode.BadRequest;
            else
                statusCode = HttpStatusCode.InternalServerError;

            var isServerError = (int)statusCode >= 500;
            var exposeDetails = ExposeErrorDetails;

            if (isServerError)
                LogError(context, exception);

            var body = new Dictionary<string, object>
            {
                ["Status"] = (int)statusCode,
                ["Title"] = GetTitle(statusCode),
                // Errores 4xx: el mensaje es intencional para el usuario, siempre se muestra.
                // Errores 5xx: solo se muestra el mensaje real si ExposeErrorDetails = true.
                ["Detail"] = isServerError && !exposeDetails
                    ? GenericServerMessage
                    : exception.Message
            };

            if (exposeDetails)
                body["Debug"] = BuildExceptionChain(exception);

            context.Response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"),
                ReasonPhrase = isServerError ? "Critical Exception" : "Business Exception"
            };
        } // OnException

        // PRIVATE

        private static string GetTitle(HttpStatusCode statusCode)
        {
            if (statusCode == HttpStatusCode.BadRequest) return "Bad request";
            if (statusCode == HttpStatusCode.InternalServerError) return "Internal Server Error";
            return statusCode.ToString();
        } // GetTitle

        /// <summary>
        /// Recorre la excepción y todas sus InnerException. Cada nivel lleva su propio
        /// StackTrace (el del nivel más interno apunta al origen real del error).
        /// </summary>
        private static List<object> BuildExceptionChain(Exception exception)
        {
            var chain = new List<object>();
            for (var current = exception; current != null; current = current.InnerException)
            {
                chain.Add(new
                {
                    Type = current.GetType().FullName,
                    current.Message,
                    current.StackTrace
                });
            }
            return chain;
        } // BuildExceptionChain

        /// <summary>
        /// Escribe el error completo (con inner exceptions y stack traces) en
        /// App_Data/Logs/errors-yyyyMMdd.log. App_Data no se sirve por IIS.
        /// El usuario del App Pool necesita permiso de escritura en esa carpeta.
        /// </summary>
        private static void LogError(HttpActionExecutedContext context, Exception exception)
        {
            try
            {
                var directory = HostingEnvironment.MapPath("~/App_Data/Logs");
                Directory.CreateDirectory(directory);

                var file = Path.Combine(directory, $"errors-{DateTime.UtcNow:yyyyMMdd}.log");
                var entry = $"[{DateTime.UtcNow:O}] {context.Request.Method} {context.Request.RequestUri}"
                    + Environment.NewLine + exception
                    + Environment.NewLine + Environment.NewLine;

                lock (_logLock)
                {
                    File.AppendAllText(file, entry, Encoding.UTF8);
                }
            }
            catch
            {
                // El logging nunca debe romper la respuesta al cliente
            }
        } // LogError

        //public override void OnException(HttpActionExecutedContext context)
        //{
        //    base.OnException(context);

        //    if (context.Exception is BusinessException businessException)
        //    {
        //        var statusCode = businessException.StatusCode ?? HttpStatusCode.BadRequest;                
        //        var validation = new { 
        //            Status = 400,
        //            Title = "Bad request",
        //            Detail = businessException.Message
        //        };
        //        string jsonCustom = JsonConvert.SerializeObject(validation);

        //        throw new HttpResponseException(new HttpResponseMessage(statusCode)
        //        {
        //            StatusCode = statusCode,
        //            Content = new StringContent(jsonCustom, Encoding.UTF8, "application/json"),
        //            ReasonPhrase = "Business Exception"
        //        });
        //    }

        //    // Log Critical errors
        //    Debug.WriteLine(context.Exception);
        //    var validationGeneral = new
        //    {
        //        Status = 500,
        //        Title = "Internal Server Error",
        //        Detail = context.Exception.Message
        //    };
        //    string jsonError = JsonConvert.SerializeObject(validationGeneral);
        //    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        //    {
        //        //Content = new StringContent("An error occurred, please try again or contact the administrator."),
        //        Content = new StringContent(jsonError, Encoding.UTF8, "application/json"),
        //        ReasonPhrase = "Critical Exception"
        //    });
        //} // OnException
    }
}