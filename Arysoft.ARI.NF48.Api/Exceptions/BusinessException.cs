using System;
using System.Linq;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;


namespace Arysoft.ARI.NF48.Api.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException() { }

        public BusinessException(string message) : base(message) { }
    }
}