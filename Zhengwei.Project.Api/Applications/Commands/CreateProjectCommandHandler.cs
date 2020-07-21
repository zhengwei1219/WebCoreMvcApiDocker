using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zhengwei.Project.Domain.AggregatesModel;

namespace Zhengwei.Project.Api.Applications.Commands
{
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Zhengwei.Project.Domain.AggregatesModel.Project>
    {
        private IProjectRepository _projectRepository;
        public CreateProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public async Task<Domain.AggregatesModel.Project> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
           _projectRepository.Add(request.Project);
           await _projectRepository.UnitOfWork.SaveEntitiesAsync();
            return request.Project;
        }
    }
}
