using Arysoft.ARI.NF48.Api.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;

namespace Arysoft.ARI.NF48.Api.Invokers
{
    public class MyApiControllerActionInvoker : ApiControllerActionInvoker
    {
        public override Task<HttpResponseMessage> InvokeActionAsync(
            HttpActionContext actionContext,
            CancellationToken cancellationToken)
        { 
            var result = base.InvokeActionAsync(actionContext, cancellationToken);

            if(result.Exception != null && result.Exception.GetBaseException() != null)
            {
                var baseException = result.Exception.GetBaseException();
                if (baseException is BusinessException)
                {
                    return Task.Run<HttpResponseMessage>(() =>
                        new HttpResponseMessage(HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent(baseException.Message),
                            ReasonPhrase = "Business Error"
                        });
                }
                else {
                    // Log critical error
                    Debug.WriteLine(baseException);

                    return Task.Run<HttpResponseMessage>(() =>
                        new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            Content = new StringContent(baseException.Message),
                            ReasonPhrase = "Critical error"
                        });
                }
            }

            return result;
        }
    }
}