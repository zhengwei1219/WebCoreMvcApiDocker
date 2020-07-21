using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Resilience;

namespace Zhengwei.Identity.Code
{
    public class ResilienceClientFactory
    {
        private ILogger<ResilienceHttpClient> _logger;
        private IHttpContextAccessor _httpContextAccessor;
        private int _retryCount;
        private int _exceptionCountBreaking;
        public ResilienceClientFactory(int exceptionCountBreaking,int retryCount,ILogger<ResilienceHttpClient> logger, IHttpContextAccessor httpContextAccessor)
        {
            _exceptionCountBreaking = exceptionCountBreaking;
            _retryCount = retryCount;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public ResilienceHttpClient GetResilienceHttpClient() =>
            new ResilienceHttpClient(origin =>CreatePolicy(origin), _logger,_httpContextAccessor);


        private Policy[] CreatePolicy(string origin)
        {
            return new Policy[]
            {
                Policy.Handle<HttpRequestException>()
                .WaitAndRetryAsync
                (_retryCount,
                retryAttempt=>TimeSpan.FromSeconds(Math.Pow(2,retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    var msg = $"第{retryCount} 次重试"+
                    $"of{context.PolicyKey} "
                    +$"at {context.ExecutionKey}, "
                    +$"due to: {exception}";
                    _logger.LogWarning(msg);
                    _logger.LogDebug(msg);
                }),
                Policy.Handle<HttpRequestException>().CircuitBreakerAsync(
                    
                    _exceptionCountBreaking,
                    TimeSpan.FromMinutes(1),
                    (excption, duration) =>
                    {
                        _logger.LogTrace("熔断器打开");
                    },()=>{
                        _logger.LogTrace("熔断器关闭");
                    })
            };
        }
    }
}
