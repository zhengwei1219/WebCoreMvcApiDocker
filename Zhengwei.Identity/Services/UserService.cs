using DnsClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Resilience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Zhengwei.Identity.Dtos;

namespace Zhengwei.Identity.Services
{

    public class UserService : IUserService
    {
        private ILogger<ResilienceHttpClient> _logger;
        //private string _userServiceUrl = "http://localhost:33545";
        private string _userServiceUrl;
        //private HttpClient _httpClient;
        private IHttpClient _httpClient;
        public UserService(IHttpClient httpClient
            ,IOptions<Dtos.ServiceDisvoveryOptions> serOp
            ,IDnsQuery dnsQuery
            , ILogger<ResilienceHttpClient> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
            
            var address  = dnsQuery.ResolveService("service.consul",serOp.Value.ServiceName);
            var addressList = address.First().AddressList;
            var host = addressList.Any() ? addressList.First().ToString() : address.First().HostName;
            var port = address.First().Port;
            _userServiceUrl = $"http://localhost:{port}"; 

        }
        public async Task<UserInfo> CheckOrCreate(string phone)
        {
            var from = new Dictionary<string, string> { { "phone", phone } };
            // var content = new FormUrlEncodedContent(from);
            try
            {
                var response = await _httpClient.PostAsync(_userServiceUrl + "/api/users/check-or-create", from, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var ret = await response.Content.ReadAsStringAsync();
                    var userInfo = JsonConvert.DeserializeObject<UserInfo>(ret);
                    return userInfo;
                }
            }
            catch (Exception ex)
            {

                _logger.LogError("checkorcreate 重试失败" + ex.Message + ex.StackTrace);
                throw ex;

            }
            
            return null;

        }
    }
}
