using DotNetCore.CAP;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zhengwei.Project.Api.Applications.IntegrationEvents;
using Zhengwei.Project.Domain.Events;

namespace Zhengwei.Project.Api.Applications.DomainEventHandlers
{
    public class ProjectCreatedDomainEventHandler:INotificationHandler<ProjectCreatedEvent>
    {
        public ICapPublisher _capPublisher;
        public ProjectCreatedDomainEventHandler(ICapPublisher capPublisher) {
            _capPublisher = capPublisher;
        }

        public Task Handle(ProjectCreatedEvent notification, CancellationToken cancellationToken)
        {
            var @event = new ProjectCreatedIntegrationEvent
            {
                ProjectId = notification.Project.Id,
                CreateTime = DateTime.Now,
                UserId = notification.Project.UserId

            };
            _capPublisher.Publish("zhengwei.projectapi.projectcreate",@event);
            return Task.CompletedTask;
        }
    }
}
