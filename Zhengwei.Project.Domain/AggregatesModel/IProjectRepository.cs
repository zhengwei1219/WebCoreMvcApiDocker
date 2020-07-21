using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zhengwei.Project.Domain.SeedWork;

namespace Zhengwei.Project.Domain.AggregatesModel
{
   public interface IProjectRepository
    {
        Task<Project> GetAsync(int id);
        Project Add(Project project);
        Project Update(Project project);
        IUnitOfWork UnitOfWork { get;}
    }
}
