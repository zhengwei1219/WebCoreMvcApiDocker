using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhengwei.Contact.Api.Dtos;

namespace Zhengwei.Contact.Api.Service
{
   public interface IUserService
    {
        Task<UserIdentity> GetBaseUserInfoAsync(int userId);
    }
}
