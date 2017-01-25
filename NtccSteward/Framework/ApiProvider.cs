using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http.Headers;

namespace NtccSteward.Framework
{
    public interface IApiProvider
    {
        T DeserializeJson<T>(string item);
        Task<string> PostItemAsync<T>(string url, T item, string queryString = null, string version = null);
        Task<string> PutItemAsync<T>(string relativeUrl, T item, string queryString = null, string version = null);
        Task<string> GetItemAsync(string relativeUrl, string queryString = null, string version = null);
        string SerializeJson<T>(T item);
        HttpClient CreateHttpClient(string version = null);
    }

    public class ApiProvider : IApiProvider
    {
        private readonly string _webApiUri;
        public ApiProvider(string webApiUri)
        {
            _webApiUri = webApiUri;
        }

        //NOTE:  NEED TO ADD GetItemAsync, As well as PutItemAsync
        //http://stackoverflow.com/questions/22505022/httpclient-getasync-post-object

        public HttpClient CreateHttpClient(string version = null)
        {
            var baseUri = _webApiUri + "/"; // changed to request.Url.Authority - this may not work.

            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUri);

            // this may not be necessary.  we just want to make sure we can deserialize json
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (version != null)
            {
                // through a custom request header
                //client.DefaultRequestHeaders.Add("api-version", version);

                // this enables interaction with a versioned Web API
                // or through content negotiation
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue($"application/vnd.ntccstewardapi.v{version}+json"));
                // "api-version":version
            }

            return client;
        }


        /// <summary>
        /// Async POST to relative url.  
        /// </summary>
        /// <param name="request">HttpRequest from controller</param>
        /// <param name="relativeUrl">/api/account/login</param>
        /// <param name="item">model to be posted</param>
        /// <param name="queryString">?id=1</param>
        /// <param name="version">Verion of WebAPI method to call, e.g., 1</param>
        public async Task<string> PostItemAsync<T>(string relativeUrl, T item, string queryString = null, string version = null)
        {
            using (var client = CreateHttpClient(version))
            {
                var response = await client.PostAsync(
                                        CreateUrl(relativeUrl, queryString), 
                                        CreateStringContent<T>(item));

                var result = ReadResponse(response);

                return result.Result;
            }
        }

        /// <summary>
        /// Async PUT to relative url.  
        /// </summary>
        /// <param name="request">HttpRequest from controller</param>
        /// <param name="relativeUrl">/api/account/login</param>
        /// <param name="item">model to be posted</param>
        /// <param name="queryString">?id=1</param>
        /// <param name="version">Verion of WebAPI method to call, e.g., 1</param>
        public async Task<string> PutItemAsync<T>(string relativeUrl, T item, string queryString = null, string version = null)
        {
            using (var client = CreateHttpClient(version))
            {
                var response = await client.PutAsync(
                                        CreateUrl(relativeUrl, queryString), 
                                        CreateStringContent<T>(item));

                var result = ReadResponse(response);
                
                return result.Result;
            }
        }

        /// <summary>
        /// Async PUT to relative url.  
        /// </summary>
        /// <param name="request">HttpRequest from controller</param>
        /// <param name="relativeUrl">/api/account/login</param>
        /// <param name="queryString">?id=1</param>
        /// <param name="version">Verion of WebAPI method to call, e.g., 1</param>
        public async Task<string> GetItemAsync(string relativeUrl, string queryString = null, string version = null)
        {
            using (var client = CreateHttpClient(version))
            {
                var response = await client.GetAsync(CreateUrl(relativeUrl, queryString));

                var result = ReadResponse(response);

                return result.Result;
            }
        }

        public string CreateUrl(string relativeUrl, string queryString = null)
        {
            var q = !string.IsNullOrEmpty(queryString) ? $"?{queryString.TrimStart('?')}" : "";
            var requestUri = relativeUrl + q;
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

        /// <summary>
        /// Creates StringContent containing serialized Json
        /// </summary>
        public StringContent CreateStringContent<T>(T item)
        {
            var json = SerializeJson<T>(item);
            var content = new StringContent(json, System.Text.Encoding.Unicode, "application/json");
            return content;
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
