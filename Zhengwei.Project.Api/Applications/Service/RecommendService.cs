using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zhengwei.Project.Api.Applications.Service
{
    public class RecommendService : IRecommendService
    {
        public Task<bool> IsProjectInRecommend(int projectId, int userId)
        {
            return Task.FromResult(true);
        }
    }
}
