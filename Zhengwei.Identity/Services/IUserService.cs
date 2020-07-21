using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhengwei.Identity.Dtos;

namespace Zhengwei.Identity.Services
{
    public interface IUserService
    {
        /// <summary>
        /// 检查手机是否注册，如果没有就创建
        /// </summary>
        /// <param name="phone"></param>
        Task<UserInfo> CheckOrCreate(string phone);
    }
}
