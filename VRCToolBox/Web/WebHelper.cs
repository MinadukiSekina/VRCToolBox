using System;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Web
{
    internal class WebHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        internal static HttpClient HttpClient { get { return _httpClient; } }

        internal static async Task<string> GetContentStringAsync(string uri)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri))
            using(HttpResponseMessage response = await _httpClient.SendAsync(request))
            {
                if(response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        internal static async Task<Stream?> GetContentStreamAsync(string uri)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri))
            using(HttpResponseMessage response = await _httpClient.SendAsync(request))
            {
                if(response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStreamAsync();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
