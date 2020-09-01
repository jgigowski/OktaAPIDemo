using System.Web.Mvc;
using System.Web.Security;
using OktaAPI.Helpers;
using OktaAPIShared.Models;
using System.Web.Configuration;

namespace OktaCustomerUI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        // GET: /Account/Widget
        [AllowAnonymous]
        public ActionResult Widget()
        {
            ViewBag.OrgUrl = $"https://{WebConfigurationManager.AppSettings["okta:BaseUrl"]}/";
            ViewBag.OAuthRedirectUri = WebConfigurationManager.AppSettings["okta:OAuthRedirectUri"];
            ViewBag.OAuthIssuerId = WebConfigurationManager.AppSettings["okta:OAuthIssuerId"];
            ViewBag.OAuthClientId = WebConfigurationManager.AppSettings["okta:OAuthClientId"];

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string BasicLogin, string OIDCLogin)
        {
            //handle 2 ways to login
            if (BasicLogin != null)
            {
                return RedirectToAction("BasicLogin", model);
            }
            else//OIDCLogin
            {
                return RedirectToAction("OIDCLogin", model);
            }
        }
        
        [AllowAnonymous]
        public ActionResult BasicLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var sessionResponse = APIHelper.SendBasicLogin(model);

            if (sessionResponse != null && !string.IsNullOrEmpty(sessionResponse.SessionToken))
            {
                var name = "Unknown";
                try
                {
                    name = $"{sessionResponse._embedded.user.profile.firstName} {sessionResponse._embedded.user.profile.lastName}";
                }
                catch
                {
                    // ignored
                }

                FormsAuthentication.SetAuthCookie(name, false);
                TempData["Message"] = "You have been logged in as " + name;
                TempData["IsError"] = false;
            }
            else
            {
                try
                {
                    var sError = sessionResponse.errorSummary;
                    if (string.IsNullOrEmpty(sError))
                    {
                        sError = sessionResponse.status;
                    }

                    TempData["Message"] = sError;
                    TempData["IsError"] = true;
                }
                catch
                {
                    // ignored
                }

            }

            //return View(model);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult OIDCLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var sessionResponse = APIHelper.SendBasicLogin(model);// This will return a Okta Session w/Session Token

            if (sessionResponse != null && !string.IsNullOrEmpty(sessionResponse.SessionToken))
            {
                var url = APIHelper.GetAuthorizationURL(sessionResponse.SessionToken);
                return Redirect(url);
            }
            else
            {
                try
                {
                    var sError = sessionResponse.errorSummary;
                    if (string.IsNullOrEmpty(sError)) {
                        sError = sessionResponse.status;
                    }
                    
                    TempData["Message"] = sError;
                    TempData["IsError"] = true;
                }
                catch
                {
                    // ignored
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        //OIDC login and Widget both redirect here (leave off the HttpGet and the HttpPost)
        public ActionResult AuthCode(string code, string state)//state code from okta should match what I sent in
        {
            var tokenresponse = APIHelper.GetToken(code);

            if (tokenresponse == null || string.IsNullOrEmpty(tokenresponse.AccessToken) || string.IsNullOrEmpty(tokenresponse.IDToken))
            {
                try
                {
                    var errordesc = "Unknown Error - No Access or ID Tokens";
                    errordesc = tokenresponse.errorSummary;
                    TempData["Message"] = errordesc;
                    TempData["IsError"] = true;
                    return RedirectToAction("Index", "Home");
                }
                catch
                {
                    // ignored
                }
            }
            else
            {
                Helpers.LoginHelper.SetOIDCTokens(tokenresponse);
            }

            return RedirectToAction("Index", "Home");
        }
      
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult OIDCLogOff()
        {
            Helpers.LoginHelper.OIDCLogout();

            ViewBag.OrgUrl = $"https://{WebConfigurationManager.AppSettings["okta:BaseUrl"]}/";

            return View("LogOff");
        }
    }
}