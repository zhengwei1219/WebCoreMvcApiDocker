using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Wrap;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Newtonsoft.Json;

namespace Resilience
{
    public class ResilienceHttpClient : IHttpClient
    {
        //根据url origin去创建policy
        private HttpClient _httpClient;
        //把policy打包成组合policy wraper,进行本地缓存。
        private readonly Func<string, IEnumerable<Policy>> _policyCreator;
        private readonly ConcurrentDictionary<string, PolicyWrap> _policyWraps;
        private ILogger<ResilienceHttpClient> _logger;
        private IHttpContextAccessor _httpContextAccessor;

        public ResilienceHttpClient(Func<string, IEnumerable<Policy>> policyCreator,
            ILogger<ResilienceHttpClient> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new HttpClient();
            _policyWraps = new ConcurrentDictionary<string, PolicyWrap>();
            _policyCreator = policyCreator;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<HttpResponseMessage> PostAsync<T>(string url, T item, string authorizationToken=null, string requestId = null, string authorizationMethod = "Beare")
        {
            Func<HttpRequestMessage> func = () => CreateHttpRequestMessage(HttpMethod.Post, url, item);
            return await DoPostPutAsync(HttpMethod.Post, url, func, authorizationToken, requestId, authorizationMethod);
        }
        public async Task<HttpResponseMessage> PostAsync(string url, Dictionary<string, string> form, string authorizationToken=null, string requestId = null, string authorizationMethod = "Beare")
        {
            Func<HttpRequestMessage> func = () => CreateHttpRequestMessage(HttpMethod.Post, url, form);
            return await DoPostPutAsync(HttpMethod.Post, url,func, authorizationToken, requestId, authorizationMethod);
        }
        public Task<HttpResponseMessage> DoPostPutAsync(HttpMethod method,string url,Func<HttpRequestMessage> requestMessageAction,string authorizationToken=null,  string requestId = null, string authorizationMethod = "Beare")
        {
            if(method != HttpMethod.Post && method != HttpMethod.Put)
            {
                throw new ArgumentException("Value must be either post or put", nameof(method));
            }
            var origin = GetOriginFromUri(url);
            return HttpInvoker(origin,async () => {
                HttpRequestMessage requestMessage = requestMessageAction();

                    SetAuthorizationHeader(requestMessage);
                
                
                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }
                if(requestId != null)
                {
                    requestMessage.Headers.Add("x-requestid", requestId);
                }
                var response = await _httpClient.SendAsync(requestMessage);
                if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError
                || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new HttpRequestException();
                }
                return response;
            });

        }
        private HttpRequestMessage CreateHttpRequestMessage<T>(HttpMethod method,string url,T item)
        {
            var requestMessage = new HttpRequestMessage(method, url);
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json");
            return requestMessage;
        }
        private HttpRequestMessage CreateHttpRequestMessage<T>(HttpMethod method, string url, Dictionary<string, string> form)
        {
            var requestMessage = new HttpRequestMessage(method, url);
            requestMessage.Content = new FormUrlEncodedContent(form);
            return requestMessage;
        }
        private async Task<T> HttpInvoker<T>(string origin,Func<Task<T>> action)
        {
            var normalizedOrigin = NormalizeOrigin(origin);
            if(!_policyWraps.TryGetValue(normalizedOrigin,out PolicyWrap policyWrap))
            {
                policyWrap=Policy.WrapAsync(_policyCreator(normalizedOrigin).ToArray());
                _policyWraps.TryAdd(normalizedOrigin, policyWrap);
            }
            return await policyWrap.ExecuteAsync(action, new Context(normalizedOrigin));
        }
        private static string NormalizeOrigin(string origin)
        {
            return origin?.Trim()?.ToLower();
        }
        private static string GetOriginFromUri(string uri)
        {
            var url = new Uri(uri);
            var origin = $"{url.Scheme}://{url.DnsSafeHost}:{url.Port}";
            return origin;
        }
        private void SetAuthorizationHeader(HttpRequestMessage requestMessage)
        {
            var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if(!string.IsNullOrEmpty(authorizationHeader))
            {
                requestMessage.Headers.Add("Authorization", new List<string>() { authorizationHeader });
            }
        }

        public Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer")
        {
            var origin = GetOriginFromUri(uri);
            return HttpInvoker(origin, async () =>
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

                SetAuthorizationHeader(requestMessage);

                if(authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }
                var response = await _httpClient.SendAsync(requestMessage);
                if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError
                || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new HttpRequestException();
                }
                return await response.Content.ReadAsStringAsync();
            });
            

        }

        public Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            Func<HttpRequestMessage> func = () => CreateHttpRequestMessage(HttpMethod.Post, uri, item);
            return  DoPostPutAsync(HttpMethod.Put, uri, func, authorizationToken, requestId, authorizationMethod);
        }
    }
}
