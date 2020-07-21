using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Resilience
{
   public interface IHttpClient
    {
        Task<HttpResponseMessage> PostAsync<T>(string url, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Beare");
        Task<HttpResponseMessage> DoPostPutAsync(HttpMethod method, string url, Func<HttpRequestMessage> requestMessageAction, string authorizationToken = null, string requestId = null, string authorizationMethod = "Beare");
        Task<HttpResponseMessage> PostAsync(string url, Dictionary<string, string> form, string authorizationToken = null, string requestId = null, string authorizationMethod = "Beare");

        Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer");

        Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");
    }
}
