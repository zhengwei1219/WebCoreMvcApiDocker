using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Zhengwei.Project.Domain.AggregatesModel;
using Zhengwei.Project.Domain.Exceptions;

namespace Zhengwei.Project.Api.Applications.Commands
{
    public class JoinProjectCommandHandler : IRequestHandler<JoinProjectCommand,Zhengwei.Project.Domain.AggregatesModel.Project>
    {
        private IProjectRepository _projectRepository;
        public JoinProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public async Task<Domain.AggregatesModel.Project> Handle(JoinProjectCommand request, CancellationToken cancellationToken)
        {
            Zhengwei.Project.Domain.AggregatesModel.Project project = await _projectRepository.GetAsync(request.Contributor.ProjectId);
            if(project != null)
            {
                project.AddContributor(request.Contributor);
               await _projectRepository.UnitOfWork.SaveEntitiesAsync();
            }
            else
            {
                throw new ProjectDomainException($"project not found {request.Contributor.ProjectId}");
            }
            return project;
        }
    }
}
