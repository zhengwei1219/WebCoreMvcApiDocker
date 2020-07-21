using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Zhengwei.Identity.Services;
using System.Security;
using System.Security.Claims;

namespace Zhengwei.Identity.Authentication
{
    public class SmsAuthCodeValidator : IExtensionGrantValidator
    {
        private readonly IAuthCodeService _authCodeService;
        private readonly IUserService _userService;
        public SmsAuthCodeValidator(IAuthCodeService authCodeService, IUserService userService)
        {
            _authCodeService = authCodeService;
            _userService = userService;
        }
        public string GrantType => "sms_auth_code";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var phone = context.Request.Raw["phone"];
            var code = context.Request.Raw["auth_code"];
            var error = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            if(!string.IsNullOrEmpty(phone)&& !string.IsNullOrEmpty(code))
            {
                //用户检查
                _authCodeService.Validate(phone, code);
                //用户注册
                var userInfo = await _userService.CheckOrCreate(phone);
                if(userInfo ==null)
                {
                    context.Result = error;
                    return;
                }
                var claims = new Claim[]
                {
                    new Claim("name",userInfo.Name??string.Empty),
                    new Claim("company",userInfo.Company??string.Empty),
                    new Claim("title",userInfo.Title??string.Empty),
                    new Claim("avatar",userInfo.Avatar??string.Empty)
                };
                context.Result = new GrantValidationResult(userInfo.Id.ToString(), GrantType,claims);
            }
            else
            {
                context.Result = error;

            }

            
        }
    }
}
