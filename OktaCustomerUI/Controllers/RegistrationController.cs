using System.Web.Mvc;
using OktaAPI.Helpers;
using OktaAPIShared.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace OktaCustomerUI.Controllers
{
    [OktaAuthorization]
    public class RegistrationController : Controller
    {
        // GET: Registration
        public ActionResult List()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
                ViewBag.IsError = TempData["IsError"];
            }

            var sJsonResponse = APIHelper.GetAllCustomers();

            dynamic dObj = JsonConvert.DeserializeObject(sJsonResponse);

            IEnumerable<BaseCustomer> model = Enumerable.Empty<BaseCustomer>();

            var sErrorSummary = "";
            try
            {
                sErrorSummary = dObj.errorSummary;
            }
            catch
            {

            }

            if (string.IsNullOrEmpty(sErrorSummary))
            {
                model = JsonConvert.DeserializeObject<IEnumerable<BaseCustomer>>(sJsonResponse);
            }
            else
            {
                ViewBag.Message = sErrorSummary;
                ViewBag.IsError = true;
            }
            
            return View("List", model);
        }

        public ActionResult Register()
        {
            ViewBag.IsNewUser = true;

            var model = new Customer();

            return View("Register", model);
        }

        public ActionResult EditUser(string Id)
        {
            ViewBag.IsNewUser = false;

            var customer = APIHelper.GetCustomerById(Id);

            return View("Register", customer);
        }

        public ActionResult DeleteUser(string Id)
        {
            APIHelper.DeleteCustomer(Id);

            TempData["Message"] = "User has been deleted";
            TempData["IsError"] = false;

            return RedirectToAction("List");
        }

        public ActionResult UpdateUser(Customer model)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", model);
            }
            
            var response = APIHelper.UpdateCustomer(model);

            if (response != null && response.errorSummary == null)
            {
                var name = "Unknown";
                try
                {
                    name = $"{model.Profile.FirstName} {model.Profile.LastName}";
                    TempData["Message"] = name + " has been updated";
                    TempData["IsError"] = false;
                }
                catch
                {

                }
            }
            else
            {
                TempData["Message"] = response.errorSummary;
                TempData["IsError"] = true;
            }

            return RedirectToAction("List");
        }

        public ActionResult AddUser(Customer model)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", model);
            }
            
            var response = APIHelper.AddNewCustomer(model);

            if (response != null && response.errorSummary == null)
            {
                var name = "Unknown";
                try
                {
                    name = $"{model.Profile.FirstName} {model.Profile.LastName}";
                    TempData["Message"] = name + " has been registered";
                    TempData["IsError"] = false;
                }
                catch
                {

                }
            }
            else
            {
                TempData["Message"] = response.errorSummary;
                TempData["IsError"] = true;
            }
            
            return RedirectToAction("List");
        }

        public ActionResult ListApps(string Id)
        {
            var response = APIHelper.ListApps(Id);

            if (response == null)
            {
                TempData["Message"] = "Unhandled Error";
                TempData["IsError"] = true;
            }
            else if (response.Count() < 1)
            {
                TempData["Message"] = "User has no applications assigned";
                TempData["IsError"] = true;
            }

            return View("Apps", response);
        }
    }
}