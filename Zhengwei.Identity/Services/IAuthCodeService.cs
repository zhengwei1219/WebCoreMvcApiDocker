using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zhengwei.Identity.Services
{
    public interface IAuthCodeService
    {
        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="authCone">验证码</param>
        /// <returns></returns>
        bool Validate(string phone, string authCone);
    }
}
