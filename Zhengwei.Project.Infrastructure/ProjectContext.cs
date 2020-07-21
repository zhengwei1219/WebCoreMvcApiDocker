using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zhengwei.Project.Domain.SeedWork;
using Zhengwei.Project.Infrastructure.EntityConfiguration;

namespace Zhengwei.Project.Infrastructure
{
    public class ProjectContext : DbContext, IUnitOfWork
    {
        IMediator _mediator;
        public DbSet<Zhengwei.Project.Domain.AggregatesModel.Project> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProjectEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectPropertyEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectContributorEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectVisibleRuleEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectViewerEntityConfiguration());
        }
        
        public ProjectContext(IMediator mediator,DbContextOptions<ProjectContext> options):base(options)
        {
            _mediator = mediator;
        }
        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _mediator.DispatchDomainEventsAsync(this);
            await base.SaveChangesAsync();
            return true;
        }
    }
}
