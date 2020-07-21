using System;
using System.Collections.Generic;
using System.Text;

namespace Zhengwei.Project.Domain.Exceptions
{
    public class ProjectDomainException:Exception
    {
        public ProjectDomainException() { }
        public ProjectDomainException(string msg):base(msg) { }
        public ProjectDomainException(string msg,Exception inner) : base(msg,inner) { }
    }
}
