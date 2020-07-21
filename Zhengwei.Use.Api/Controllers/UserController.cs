using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Zhengwei.Use.Api.Filters;
using Zhengwei.UserApi.Data;
using Microsoft.AspNetCore.JsonPatch;
using Zhengwei.UserApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using DotNetCore.CAP;

namespace Zhengwei.Use.Api.Controllers
{
    [Route("api/users")]
    public class UserController : BaseController
    {
        public ICapPublisher _capPublisher;
        public UserContext _userContext;
        public UserController(UserContext userContext, ICapPublisher capPublisher)
        {
            _userContext = userContext;
            _capPublisher = capPublisher;
        }
        // GET api/values
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user =_userContext.Users.SingleOrDefault(u=>u.Id == UserIdentity.UserId);
            if(user == null)
            {
                throw new UserOperationException("获取用户数据出错，用户ID:"+UserIdentity.UserId);

            }
            return Json(user);
        }

        public void RaiseUserprofileChangedEvent(AppUser user)
        {
            if(_userContext.Entry(user).Property(nameof(user.Name)).IsModified ||
               _userContext.Entry(user).Property(nameof(user.Title)).IsModified ||
               _userContext.Entry(user).Property(nameof(user.Company)).IsModified ||
               _userContext.Entry(user).Property(nameof(user.Avatar)).IsModified ||
               _userContext.Entry(user).Property(nameof(user.Phone)).IsModified )
            {
                _capPublisher.Publish("zhengwei.userapi.userchanged",new Dtos.UserIdentity {
                  Name = user.Name,
                  Title = user.Title,
                  Avatar = user.Avatar,
                  Company = user.Company
                });
            }
        }
        [Route("")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]JsonPatchDocument<AppUser> userPatch)
        {
            var user = _userContext.Users
                                    .SingleOrDefault(u => u.Id == UserIdentity.UserId);
            userPatch.ApplyTo(user);
            var originProperties = user.Properties;
            var allProperties = originProperties.Union(user.Properties).Distinct();

            var removedProperties = originProperties.Except(user.Properties);
            var newProperties = allProperties.Except(user.Properties);

            foreach (var property  in removedProperties)
            {
                _userContext.Remove(property);
            }
            foreach (var property in newProperties)
            {
                _userContext.Add(property);
            }
            using (var transaction = _userContext.Database.BeginTransaction())
            {
                RaiseUserprofileChangedEvent(user);
                _userContext.Users.Update(user);
                _userContext.SaveChanges();
                transaction.Commit();
            }
              
            return Json(user);
        }
        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [Route("")]
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [Route("")]
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        /// <summary>
        /// 检查或创建用户
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [Route("check-or-create")]
        [HttpPost]
        public async Task<IActionResult> CheckOrCreate(string phone)
        {
            var user = _userContext.Users.SingleOrDefault(u => u.Phone == phone);
            if(user == null)
            {
                user = new AppUser() { Phone = phone };
                _userContext.Users.Add(user);
                _userContext.SaveChanges();
               
            }
            return Ok(new { user.Id,user.Name,user.Company,user.Avatar});
        }

        [HttpGet]
        [Route("tags")]
        public async Task<IActionResult> GetUserTags()
        {
          return Ok(await  _userContext.UserTags.Where(s=>s.UserId == UserIdentity.UserId).ToListAsync());
        }
        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search(string phone)
        {
           return Ok(await _userContext.Users.Include(s => s.Properties).SingleOrDefaultAsync(s => s.Id == UserIdentity.UserId));
        }
        [HttpPut]
        [Route("tags")]
        public async Task<IActionResult> UpdateUserTas([FromBody]List<string> tags)
        {
            var originTags = await _userContext.UserTags.Where(u => u.UserId == UserIdentity.UserId).Select(t=>t.Tag).ToListAsync();
            var newtags =tags.Except(originTags);
            await _userContext.UserTags.AddRangeAsync(newtags.Select(t => new UserTag {
                CreatedTime = DateTime.Now,
                UserId = UserIdentity.UserId,
                Tag = t
            }));
            await _userContext.SaveChangesAsync();
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetBaseInfo(int userId)
        {
            var user = await _userContext.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if(user==null)
            {
                return NotFound();
            }

            return Ok(new
            {
               userId = user.Id,
                user.Name,
                user.Company,
                user.Title,
                user.Avatar
            });
        }
    }
}
