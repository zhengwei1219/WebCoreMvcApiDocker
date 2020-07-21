using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhengwei.Recommend.Api.Dtos;

namespace Zhengwei.Recommend.Api.Service
{
   public interface IUserService
    {
        Task<UserIdentity> GetBaseUserInfoAsync(int userId);
    }
}
