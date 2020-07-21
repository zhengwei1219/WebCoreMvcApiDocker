using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zhengwei.Project.Domain.AggregatesModel;
using Zhengwei.Project.Domain.SeedWork;
using ProjectEntity = Zhengwei.Project.Domain.AggregatesModel.Project;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Zhengwei.Project.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ProjectContext _projectContext;

        IUnitOfWork IProjectRepository.UnitOfWork => _projectContext;

        public ProjectRepository(ProjectContext projectContext)
        {
            _projectContext = projectContext;
        }
        public ProjectEntity Add(ProjectEntity project)
        {
           return _projectContext.Add(project).Entity;
        }

        public async Task<ProjectEntity> GetAsync(int id)
        {
          var project = await  _projectContext.Projects
                           .Include(p => p.Properties)
                           .Include(p => p.Viewers)
                           .Include(p => p.Contributors)
                           .Include(p => p.VisibleRule)
                           .SingleOrDefaultAsync();
            return project;
        }

        public ProjectEntity Update(ProjectEntity project)
        {
            return _projectContext.Update(project).Entity;
        }
    }
}
