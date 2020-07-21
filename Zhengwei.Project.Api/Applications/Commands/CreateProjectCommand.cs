using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhengwei.Project.Domain;

namespace Zhengwei.Project.Api.Applications.Commands
{
    public class CreateProjectCommand:IRequest<Zhengwei.Project.Domain.AggregatesModel.Project>
    {
        public Domain.AggregatesModel.Project Project { get; set; }
    }
}
