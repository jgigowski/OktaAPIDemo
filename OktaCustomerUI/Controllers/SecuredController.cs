using System.Web.Mvc;

namespace OktaCustomerUI.Controllers
{
    
    public class SecuredController : Controller
    {
        [OktaAuthorization]
        public ActionResult Index()
        {
            return View();
        }
    }
}
