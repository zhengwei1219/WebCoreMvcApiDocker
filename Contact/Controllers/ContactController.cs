using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Zhengwei.Contact.Api.Controllers;
using Zhengwei.Contact.Api.Data;
using Zhengwei.Contact.Api.Dtos;
using Zhengwei.Contact.Api.Service;
using Zhengwei.Contact.Api.ViewModel;

namespace Zhengwei.Contact.Controllers
{
    [Route("api/contacts")]
    public class ContactController : BaseController
    {
        private IContactApplyRequestRepository _contactApplyRequestRepository;
        private IUserService _userService;
        private IContactRepository _contactReository;
        public ContactController(IUserService userService,IContactApplyRequestRepository contactApplyRequestRepository,IContactRepository contactReository)
        {
            _userService = userService;
            _contactReository = contactReository;
            _contactApplyRequestRepository = contactApplyRequestRepository;
        }
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get(CancellationToken token)
        {
            return Ok(await _contactReository.GetContactsAsync(UserIdentity.UserId, token));
        }
        [HttpPut]
        [Route("tag")]
        public async Task<IActionResult> TagContact([FromBody]TagContactInputViewModel viewModel,CancellationToken token)
        {
            return Ok(await _contactReository.TagContactAsync(UserIdentity.UserId,viewModel.ContactId,viewModel.Tags, token));
        }
        /// <summary>
        /// 获取好友申请列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("apply-requests")]
        public async Task<IActionResult> GetApplyRequests(CancellationToken token)
        {
            var requests = await _contactApplyRequestRepository.GetRequestListAsync(UserIdentity.UserId, token);
            return Ok(requests);
        }
        /// <summary>
        /// 添加好友请求
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("apply-requests")]
        public async Task<IActionResult> AddApplyRequest(int userId)
        {
      
           var ret = await _contactApplyRequestRepository.AddRequestAsync(new Api.Models.ContactApplyRequest
            {
                UserId = userId,
                ApplierId = UserIdentity.UserId,
                Name = UserIdentity.Name,
                Company = UserIdentity.Company,
                Title = UserIdentity.Title,
                Avatar = UserIdentity.Avatar,
                CreatedTime = DateTime.Now

            }, CancellationToken.None);
            if(!ret)
            {
                return BadRequest();
            }
            return Ok();
        }
        /// <summary>
        /// 添加好友请求
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("apply-requests")]
        public async Task<IActionResult> ApprovalApplyRequest(int userId,int applierId, CancellationToken token)
        {
            var request = await _contactApplyRequestRepository.ApprovalAsync(userId,applierId, token);
            if (!request)
            {
                return BadRequest();
            }
            UserIdentity applier = await _userService.GetBaseUserInfoAsync(applierId);
            await _contactReository.AddContact(UserIdentity.UserId, applier, token);

            UserIdentity userInfo = await _userService.GetBaseUserInfoAsync(UserIdentity.UserId);
            await _contactReository.AddContact(applierId, userInfo, token);
            return Ok();

        }
    }
}
