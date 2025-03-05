using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class DisclaimerController : Controller
    {
        // GET: Disclaimer
        public ActionResult Index()
        {
            ViewBag.Title = "Disclaimer";

            return View();
        }
    }
}