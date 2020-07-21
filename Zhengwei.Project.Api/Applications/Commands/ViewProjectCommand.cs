﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zhengwei.Project.Api.Applications.Commands
{
    public class ViewProjectCommand: IRequest<bool>
    {
        public int ProjectId { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
    }
}
