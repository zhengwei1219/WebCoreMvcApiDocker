using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zhengwei.Identity.Services
{
    public class AuthCodeService : IAuthCodeService
    {
        public bool Validate(string phone, string authCone)
        {
            return true;
        }
    }
}
