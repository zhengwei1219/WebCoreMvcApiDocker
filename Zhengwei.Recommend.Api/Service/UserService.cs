using DnsClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Resilience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhengwei.Recommend.Api.Dtos;

namespace Zhengwei.Recommend.Api.Service
{
    public class UserService : IUserService
    {
        private ILogger<UserService> _logger;
        private string _userServiceUrl;
        private IHttpClient _httpClient;
        public UserService(IHttpClient httpClient
            , IOptions<Dtos.ServiceDisvoveryOptions> serOp
            , IDnsQuery dnsQuery
            , ILogger<UserService> logger)
        {
            _logger = logger;
            _httpClient = httpClient;

            var address = dnsQuery.ResolveService("service.consul", serOp.Value.ServiceName);
            var addressList = address.First().AddressList;
            var host = addressList.Any() ? addressList.First().ToString() : address.First().HostName;
            var port = address.First().Port;
            _userServiceUrl = $"http://localhost:{port}";

        }
       
        public async Task<UserIdentity> GetBaseUserInfoAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(_userServiceUrl + "/api/users/baseinfo/"+userId);
                if (string.IsNullOrEmpty(response))
                {
                    var userInfo = JsonConvert.DeserializeObject<UserIdentity>(response);
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
