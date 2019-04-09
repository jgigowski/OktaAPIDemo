using System.Web;
using System;
using OktaAPIShared.Models;
using OktaAPI.Helpers;

namespace OktaCustomerUI.Helpers
{
    public static class LoginHelper
    {
        public static readonly string ACCESS_TOKEN = "ACCESS_TOKEN";//auth token for introspection and access at okta level
        public static readonly string ID_TOKEN = "ID_TOKEN";//id token for app to parse and look at claims

        //For demonstration purpose only
        public static readonly string VERIFIED_REQUEST = "VERIFIED_REQUEST";//only do once per request
        public static readonly string TIR = "TIR";

        public static bool CheckedRequest()
        {
            bool bChecked = false;
            if (HttpContext.Current.Items[VERIFIED_REQUEST] != null && (bool)HttpContext.Current.Items[VERIFIED_REQUEST]) {
                bChecked = true;
            }

            HttpContext.Current.Items[VERIFIED_REQUEST] = true;//set for next time

            return bChecked;
        }

        public static void SetOIDCTokens(OIDCTokenResponse tokenresponse)
        {
            HttpContext.Current.Items[VERIFIED_REQUEST] = false;//reset to recheck

            var accesscookie = GetAccessCookie();
            accesscookie.Value = tokenresponse.AccessToken;
            accesscookie.Expires = DateTime.Now.AddDays(1);
            HttpContext.Current.Response.Cookies.Add(accesscookie);

            //Not Used
            //var idcookie = new HttpCookie(ID_TOKEN);
            //idcookie.Value = tokenresponse.IDToken;
            //idcookie.Expires = DateTime.Now.AddDays(1);
            //HttpContext.Current.Response.Cookies.Add(idcookie);
        }

        public static bool IsUserAuthorized()
        {
            var oTIR = GetOIDCIntrospectionDetails();
            return oTIR != null;
        }

        public static TokenIntrospectionResponse GetOIDCIntrospectionDetails()
        {
            if (CheckedRequest())
            {
                return (TokenIntrospectionResponse)HttpContext.Current.Items[TIR];
            }

            TokenIntrospectionResponse oTIR = null;

            var accesscookie = GetAccessCookie();
            accesscookie = HttpContext.Current.Request.Cookies[ACCESS_TOKEN];

            if (accesscookie != null)
            {
                string accesstoken = accesscookie.Value;

                if (!string.IsNullOrEmpty(accesstoken))
                {
                    oTIR = APIHelper.IntrospectToken(accesstoken);

                    if (oTIR == null || !oTIR.Active)
                    {
                        oTIR = null;
                        ExpireCookies();
                    }
                }
            }

            HttpContext.Current.Items[TIR] = oTIR;

            return oTIR;
        }
        
        private static void ExpireCookies()
        {
            var accesscookie = GetAccessCookie();
            accesscookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(accesscookie);

            //Not Used
            //var idcookie = new HttpCookie(ID_TOKEN);
            //idcookie.Expires = DateTime.Now.AddDays(-1);
            //HttpContext.Current.Response.Cookies.Add(idcookie);
        }

        private static HttpCookie GetAccessCookie()
        {
            return new HttpCookie(ACCESS_TOKEN);
        }

        private static string GetAccessToken()
        {
            var accesscookie = GetAccessCookie();
            accesscookie = HttpContext.Current.Request.Cookies[ACCESS_TOKEN];

            if (accesscookie != null)
            {
                return accesscookie.Value;
            }
            return "";
        }

        public static void OIDCLogout()
        {
            var AccessToken = GetAccessToken();
            if (!string.IsNullOrEmpty(AccessToken))
            {
                APIHelper.RevokeToken(AccessToken);
            }
            
            HttpContext.Current.Items[VERIFIED_REQUEST] = null;
            HttpContext.Current.Items[TIR] = null;

            ExpireCookies();
        }
    }
}