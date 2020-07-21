using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zhengwei.Recommend.Api.IntegrationEvents
{
    public class ProjectCreatedIntegrationEvent
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public DateTime CreateTime { get; set; }
        public string Company { get; set; }

        public string Introduction { get; set; }
    }
}
