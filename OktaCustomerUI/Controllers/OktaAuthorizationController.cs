using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OktaCustomerUI.Controllers
{
    public class OktaAuthorizationAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return Helpers.LoginHelper.IsUserAuthorized();
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(
                    new
                    {
                        controller = "Home",
                        action = "Unauthorized"
                    })
                );
        }
    }
}