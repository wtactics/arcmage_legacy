using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace WTacticsService.Controllers
{
    public class LogController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
