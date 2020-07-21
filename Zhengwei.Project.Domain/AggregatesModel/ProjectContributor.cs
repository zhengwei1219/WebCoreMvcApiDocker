﻿using System;
using System.Collections.Generic;
using System.Text;
using Zhengwei.Project.Domain.SeedWork;

namespace Zhengwei.Project.Domain.AggregatesModel
{
   public class ProjectContributor:Entity
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public DateTime CreatedTime { get; set; }

        public bool IsCloser { get; set; }
        /// <summary>
        /// 1:财务顾问  2:投资机构
        /// </summary>
        public int ContributorType { get; set; }

    }
}
