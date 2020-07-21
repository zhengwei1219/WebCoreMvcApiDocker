using System;
using System.Collections.Generic;
using System.Text;

namespace Zhengwei.Project.Domain.AggregatesModel
{
   public class ProjectViewer
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
