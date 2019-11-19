using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using GeoProximityMVC;

namespace GeoProximityMVCTest.Helper
{
    internal class ApiRequestHelper
    {
        internal static HttpResponseMessage SendRequest(string uri, HttpMethod method)
        {
            if (string.IsNullOrEmpty(uri) || method == null)
                return null;

            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);

            HttpResponseMessage response = null;
            using (HttpServer httpServer = new HttpServer(config))
            {
                using (HttpClient client = new HttpClient(httpServer))
                {
                    if (method == HttpMethod.Get)
                    {
                        HttpRequestMessage request = new HttpRequestMessage(method, uri);
                        response = client.SendAsync(request).Result;
                    }
                }
            }

            return response;
        }
    }
}