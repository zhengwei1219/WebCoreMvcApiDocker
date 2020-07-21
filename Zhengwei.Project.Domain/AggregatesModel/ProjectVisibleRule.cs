using System;
using System.Collections.Generic;
using System.Text;

namespace Zhengwei.Project.Domain.AggregatesModel
{
   public class ProjectVisibleRule
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public bool Visible { get; set; }
        public string Tags { get; set; }
    }
}
