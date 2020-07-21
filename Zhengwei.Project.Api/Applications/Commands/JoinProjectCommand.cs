using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zhengwei.Project.Api.Applications.Commands
{
    public class JoinProjectCommand: IRequest<Zhengwei.Project.Domain.AggregatesModel.Project>
    {
       public Domain.AggregatesModel.ProjectContributor Contributor { get; set; }
    }
}
