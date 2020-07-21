using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhengwei.Project.Domain.AggregatesModel;

namespace Zhengwei.Project.Api.Applications.IntegrationEvents
{
    public class ProjectViewerIntegrationEvent
    {
        public string Company { get; set; }
        public string Introduction { get; set; }
        public ProjectViewer Viewer { get; set; }
    }
}
