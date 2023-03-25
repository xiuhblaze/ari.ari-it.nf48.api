using System.Web;
using System.Web.Mvc;

namespace Arysoft.ARI.NF48.Api
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
