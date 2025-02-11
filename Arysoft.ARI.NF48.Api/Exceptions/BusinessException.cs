using System;


namespace Arysoft.ARI.NF48.Api.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException() { }

        public BusinessException(string message) : base(message) { }
    }
}