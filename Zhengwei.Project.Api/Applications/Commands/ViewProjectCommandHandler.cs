using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zhengwei.Project.Domain.AggregatesModel;
using Zhengwei.Project.Domain.Exceptions;

namespace Zhengwei.Project.Api.Applications.Commands
{
    public class ViewProjectCommandHandler:IRequestHandler<ViewProjectCommand,bool>
    {
        private IProjectRepository _projectRepository;
        public ViewProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

       

        async Task<bool> IRequestHandler<ViewProjectCommand, bool>.Handle(ViewProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetAsync(request.ProjectId);
            if (project == null)
            {
                throw new ProjectDomainException($"project not found {request.ProjectId}");
            }
            project.AddViewer(request.UserId, request.UserName, request.Avatar);
            return await _projectRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
