﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Zhengwei.Project.Domain.AggregatesModel;

namespace Zhengwei.Project.Domain.Events
{
   public class ProjectViewedEvent: INotification
    {
        public string Company { get; set; }
        public string Introduction { get; set; }
        public ProjectViewer Viewer { get; set; }
    }
}
