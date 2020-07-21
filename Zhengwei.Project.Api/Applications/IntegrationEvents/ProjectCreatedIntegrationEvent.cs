using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zhengwei.Project.Api.Applications.IntegrationEvents
{
    public class ProjectCreatedIntegrationEvent
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
