using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OktaCustomerUI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
                ViewBag.IsError = TempData["IsError"];
            }

            return View();
        }

        public ActionResult Unauthorized()
        {
            return View();
        }
    }
}