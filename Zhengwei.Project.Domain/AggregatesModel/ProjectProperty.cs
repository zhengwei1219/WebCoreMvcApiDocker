using System;
using System.Collections.Generic;
using System.Text;

namespace Zhengwei.Project.Domain.AggregatesModel
{
   public class ProjectProperty
    {
        public int ProjectId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public ProjectProperty(string key,string text,string value)
        {
            this.Key = key;
            this.Text = text;
            this.Value = value;
        }
    }
}
