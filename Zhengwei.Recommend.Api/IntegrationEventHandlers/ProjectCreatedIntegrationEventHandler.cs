using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhengwei.Recommend.Api.Data;
using Zhengwei.Recommend.Api.IntegrationEvents;
using Zhengwei.Recommend.Api.Models;
using Zhengwei.Recommend.Api.Service;

namespace Zhengwei.Recommend.Api.IntegrationEventHandlers
{
    public class ProjectCreatedIntegrationEventHandler:ICapSubscribe
    {
        private RecommendDbContext _context;
        private IUserService _userService;
        public ProjectCreatedIntegrationEventHandler(RecommendDbContext context)
        {
            _context = context;
        }
        public async Task CreatRecommendFromProject(ProjectCreatedIntegrationEvent @event)
        {
            var fromUser = await _userService.GetBaseUserInfoAsync(@event.UserId);
            var recommend = new ProjectRecommend()
            {
                Company = @event.Company,
                CreateTime = @event.CreateTime,
                ProjectId = @event.ProjectId,
                Introduction = @event.Introduction,
                RecommendTime = DateTime.Now,
                RecommendType = EnumRecommendType.Friend,
                UserId = @event.UserId,
                FromUserAvatar = fromUser.Avatar,
                FromUserName = fromUser.Name
                
            };
            _context.ProjectRecommends.Add(recommend);
            _context.SaveChanges();
        }
    }
}
