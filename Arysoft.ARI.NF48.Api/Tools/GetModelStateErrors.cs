using System.Linq;
using System.Web.Http.ModelBinding;

namespace Arysoft.ARI.NF48.Api.Tools
{
    public partial class Strings
    {
        public static string GetModelStateErrors(ModelStateDictionary modelState)
        {
            var message = string.Join(" | ", modelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => string.IsNullOrEmpty(e.ErrorMessage)
                        ? e.Exception?.Message 
                        : e.ErrorMessage));
            
            return message;
        } // GetModelStateErrors
    }
}