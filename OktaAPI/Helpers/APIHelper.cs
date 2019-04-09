using System;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using System.Web.Configuration;
using Newtonsoft.Json;
using OktaAPIShared.Models;

namespace OktaAPI.Helpers
{
    public class APIHelper
    {
        private static string _apiUrlBase;
        private static string _oktaToken;
        private static string _oktaOAuthHeaderAuth;
        private static string _oktaOAuthIssuerId;
        private static string _oktaOAuthClientId;
        private static string _oktaOAuthRedirectUri;
        
        private static HttpClient _client = new HttpClient();

        static APIHelper()
        {
            _apiUrlBase = WebConfigurationManager.AppSettings["okta:BaseUrl"];
            _oktaToken = WebConfigurationManager.AppSettings["okta:APIToken"];

            _oktaOAuthIssuerId = WebConfigurationManager.AppSettings["okta:OAuthIssuerId"];
            _oktaOAuthClientId = WebConfigurationManager.AppSettings["okta:OAuthClientId"];
            _oktaOAuthRedirectUri = WebConfigurationManager.AppSettings["okta:OAuthRedirectUri"];
            
            var oktaOAuthSecret = WebConfigurationManager.AppSettings["okta:OauthClientSecret"];
            _oktaOAuthHeaderAuth = Base64Encode($"{_oktaOAuthClientId}:{oktaOAuthSecret}");
        }

        public static string GetAllCustomers()
        {
            var sJsonResponse = JsonHelper.Get($"https://{_apiUrlBase}/api/v1/users?limit=100", _oktaToken);
            return sJsonResponse;
        }

        public static Customer GetCustomerById(string Id)
        {
            var sJsonResponse = JsonHelper.Get($"https://{_apiUrlBase}/api/v1/users/{Id}", _oktaToken);
            return JsonConvert.DeserializeObject<Customer>(sJsonResponse);
        }

        public static dynamic UpdateCustomer(Customer oCustomer)
        {
            var uc = new CustomerUpdate(oCustomer);
            var sJsonResponse = JsonHelper.Post($"https://{_apiUrlBase}/api/v1/users/{oCustomer.Id}", JsonHelper.JsonContent(uc), _oktaToken);
            return JsonConvert.DeserializeObject(sJsonResponse);
        }

        public static dynamic AddNewCustomer(Customer oCustomer)
        {
            var oNewCustomer = new CustomerAdd(oCustomer);
            var sJsonResponse = JsonHelper.Post($"https://{_apiUrlBase}/api/v1/users?activate=true", JsonHelper.JsonContent(oNewCustomer), _oktaToken);
            return JsonConvert.DeserializeObject(sJsonResponse);
        }

        public static void DeleteCustomer(string Id)
        {
            //Deactivate - 1st time to send the delete will deactivate
            JsonHelper.Delete($"https://{_apiUrlBase}/api/v1/users/{Id}", _oktaToken);

            //Delete - 2nd time to send delete will delete
            JsonHelper.Delete($"https://{_apiUrlBase}/api/v1/users/{Id}", _oktaToken);
        }
        
        public static string GetAuthorizationURL(string oktaSessionToken)
        {
            return $"https://{_apiUrlBase}/oauth2/{_oktaOAuthIssuerId}/v1/authorize?response_type=code&client_id={_oktaOAuthClientId}&redirect_uri={_oktaOAuthRedirectUri}&scope=openid&state=af0ifjsldkj&nonce=n-0S6_WzA2Mj&sessionToken={oktaSessionToken}";
        }

        public static OIDCTokenResponse GetToken(string oktaAuthCode)
        {
            var sJsonResponse = JsonHelper.Post($"https://{_apiUrlBase}/oauth2/{_oktaOAuthIssuerId}/v1/token?code={oktaAuthCode}&grant_type=authorization_code&redirect_uri={_oktaOAuthRedirectUri}", null, null, _oktaOAuthHeaderAuth);
            return JsonConvert.DeserializeObject<OIDCTokenResponse>(sJsonResponse);
        }
        
        public static OktaSessionResponse SendBasicLogin(LoginViewModel login)
        {
            //create simple class to lowecase & minimize model for json - case sensitive
            var ologin = new Login
            {
                username = login.UserName,
                password = login.Password
            };

            var sJsonResponse = JsonHelper.Post($"https://{_apiUrlBase}/api/v1/authn", JsonHelper.JsonContent(ologin));
            return JsonConvert.DeserializeObject<OktaSessionResponse>(sJsonResponse);
        }

        public static TokenIntrospectionResponse IntrospectToken(string token)
        {
            var sJsonResponse = JsonHelper.Post($"https://{_apiUrlBase}/oauth2/{_oktaOAuthIssuerId}/v1/introspect?token={token}&token_type_hint=access_token", null, null, _oktaOAuthHeaderAuth);
            if (string.IsNullOrEmpty(sJsonResponse))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<TokenIntrospectionResponse>(sJsonResponse);
        }

        public static List<App> ListApps(string Id)
        {
            var sJsonResponse = JsonHelper.Get($"https://{_apiUrlBase}/api/v1/apps?filter=user.id+eq+\"{Id}\"", _oktaToken);
            //var sJsonResponse = JsonHelper.Get($"https://{_apiUrlBase}/api/v1/apps?filter=user.id+eq+\"{Id}\"&expand=user/{Id}", _oktaToken);

            return JsonConvert.DeserializeObject<List<App>>(sJsonResponse);
        }
        
        public static void RevokeToken(string token)
        {
            var response = JsonHelper.Post($"https://{_apiUrlBase}/oauth2/{_oktaOAuthIssuerId}/v1/revoke?token={token}&token_type_hint=access_token", null, null,_oktaOAuthHeaderAuth);
        }

        private static string Base64Encode(string plainText)
        {
            var bytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(bytes);
        }
    }
}