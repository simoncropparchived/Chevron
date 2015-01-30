using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chevron.Mvc.V8.Sample.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var version = typeof(ChevronViewEngine).Assembly.GetName().Version;
            var model = new
            {
                ViewEngineName = "Chevron",
                ViewEngineVersion = version.ToString(3)
            };
            return View("Index",model);
        }
    }
}