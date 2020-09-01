using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;

namespace OktaAPI.Helpers
{
    public class JsonHelper
    {
        // Returns JSON string
        public static string Get(string url, string oktaToken)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = @"application/json";
            request.Accept = @"application/json";

            if (url.Contains("https:"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }

            if (!string.IsNullOrEmpty(oktaToken))
            {
                request.Headers.Add("Authorization", "SSWS " + oktaToken);
            }

            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return "Unhandled Error";
            }
        }

        // POST a JSON string
        public static string Post(string url, string jsonContent = null, string oktaToken = null, string oktaOAuthHeader = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = @"application/json";
            request.Accept = @"application/json";

            if (url.Contains("https:")) {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }

            if (!string.IsNullOrEmpty(oktaToken))
            {
                request.Headers.Add("Authorization", "SSWS " + oktaToken);
            }

            if (!string.IsNullOrEmpty(oktaOAuthHeader))
            {
                request.Headers.Add("Authorization", "Basic " + oktaOAuthHeader);
            }

            if (!string.IsNullOrEmpty(jsonContent))
            {
                UTF8Encoding encoding = new UTF8Encoding();
                Byte[] byteArray = encoding.GetBytes(jsonContent);
                request.ContentLength = byteArray.Length;

                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
            }
            else
            {
                request.ContentType = @"application/x-www-form-urlencoded";
            }
            
            try
            {
                var response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                if (ex.Response == null) {
                    return null;
                }
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                //The request was aborted: Could not create SSL/TLS secure channel.
                return null;
            }
        }

        public static string Delete(string url, string oktaToken = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "DELETE";
            request.ContentType = @"application/json";
            request.Accept = @"application/json";

            if (url.Contains("https:"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }

            if (!string.IsNullOrEmpty(oktaToken))
            {
                request.Headers.Add("Authorization", "SSWS " + oktaToken);
            }

            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return "Unhandled Error";
            }
        }

        public static string JsonContent(object model)
        {
            if (model == null) return null;
            string json;
            using (var ms = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(model.GetType());
                serializer.WriteObject(ms, model);
                ms.Position = 0;
                StreamReader sr = new StreamReader(ms);
                json = sr.ReadToEnd();
            }
            return json;
        }
    }
}