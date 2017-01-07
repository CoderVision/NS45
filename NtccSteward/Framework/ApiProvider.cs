using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace NtccSteward.Framework
{
    public interface IApiProvider
    {
        T DeserializeJson<T>(string item);
        Task<string> PostItemAsync<T>(HttpRequest request, string url, T item);
        string SerializeJson<T>(T item);
    }

    public class ApiProvider : IApiProvider
    {
        //NOTE:  NEED TO ADD GetItemAsync, As well as PutItemAsync
        //http://stackoverflow.com/questions/22505022/httpclient-getasync-post-object


        /// <summary>
        /// Async posts to relative url.  
        /// </summary>
        /// <param name="request">HttpRequest from controller</param>
        /// <param name="relativeUrl">/api/account/login</param>
        /// <param name="item">model to be posted</param>
        /// <returns></returns>
        public async Task<string> PostItemAsync<T>(HttpRequest request, string relativeUrl, T item)
        {
            var root = request.IsSecureConnection ? "https" : "http";
            var requestUri = $"{root}://{request.Url.Authority}/{relativeUrl}"; // changed to request.Url.Authority - this may not work.

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(requestUri, new StringContent(SerializeJson<T>(item)));

                var result = ReadResponse(response);

                return result.Result;
            }
        }



        public async Task<string> PutItemAsync<T>(HttpRequest request, string relativeUrl, T item)
        {
            var requestUri = CreateRequestUri(request, relativeUrl);

            using (var client = new HttpClient())
            {
                var response = await client.PutAsync(requestUri, new StringContent(SerializeJson<T>(item)));

                var result = ReadResponse(response);
                
                return result.Result;
            }
        }

        public async Task<string> GetItemAsync(HttpRequest request, string relativeUrl, string queryString)
        {
            var requestUri = CreateRequestUri(request, relativeUrl) + $"?{queryString.TrimStart('?')}"; // changed to request.Url.Authority - this may not work.

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(requestUri);

                var result = ReadResponse(response);

                return result.Result;
            }
        }

        private string CreateRequestUri(HttpRequest request, string relativeUrl)
        {
            var root = request.IsSecureConnection ? "https" : "http";
            var requestUri = $"{root}://{request.Url.Authority}/{relativeUrl}";
            return requestUri;
        }

        private async Task<string> ReadResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return $"Error processing request.  Status code:  {response.StatusCode}";
            }
        }

        public T DeserializeJson<T>(string item)
        {
            if (string.IsNullOrWhiteSpace(item))
                return default(T);

            return JsonConvert.DeserializeObject<T>(item);
        }

        public string SerializeJson<T>(T item)
        {
            return JsonConvert.SerializeObject(item);
        }
    }
}
