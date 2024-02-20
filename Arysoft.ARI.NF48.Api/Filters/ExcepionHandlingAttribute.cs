using Arysoft.ARI.NF48.Api.Exceptions;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Filters;

namespace Arysoft.ARI.NF48.Api.Filters
{
    public class ExcepionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            base.OnException(context);

            if (context.Exception is BusinessException businessException)
            {
                var validation = new { 
                    Status = 400,
                    Title = "Bad request",
                    Detail = businessException.Message
                };
                string jsonCustom = JsonConvert.SerializeObject(validation);

                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(jsonCustom, Encoding.UTF8, "application/json"),
                    ReasonPhrase = "Business Exception"
                });
            }

            // Log Critical errors
            Debug.WriteLine(context.Exception);
            var validationGeneral = new
            {
                Status = 500,
                Title = "Internal Server Error",
                Detail = context.Exception.Message
            };
            string jsonError = JsonConvert.SerializeObject(validationGeneral);
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                //Content = new StringContent("An error occurred, please try again or contact the administrator."),
                Content = new StringContent(jsonError, Encoding.UTF8, "application/json"),
                ReasonPhrase = "Critical Exception"
            });
        } // OnException
    }
}