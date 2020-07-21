using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhengwei.Project.Domain.SeedWork;

namespace Zhengwei.Project.Infrastructure
{
   public static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator,ProjectContext pc)
        {
            var domainEntities = pc.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null);

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(e => e.Entity.ClearDomainEvent());

            var tasks = domainEntities
                .Select( async  (domainEvent) => {
                    //await mediator.Publish(domainEvent);
                });
            await Task.WhenAll(tasks);
        }
    }
}
