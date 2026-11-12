using System;
using System.Net;

namespace Arysoft.ARI.NF48.Api.Exceptions
{
    public class BusinessException : Exception
    {
        public HttpStatusCode? StatusCode;

        public BusinessException() { }

        public BusinessException(string message) : base(message) { }

        public BusinessException(string message, Exception innerException)
            : base(message, innerException) { }

        public BusinessException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public BusinessException(string message, HttpStatusCode statusCode, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}