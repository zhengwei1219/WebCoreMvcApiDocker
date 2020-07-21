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
    public class ProjectJoinedDomainEventHandler : INotificationHandler<ProjectJoinedEvent>
    {
        public ICapPublisher _capPublisher;
        public ProjectJoinedDomainEventHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public Task Handle(ProjectJoinedEvent notification, CancellationToken cancellationToken)
        {
            var @event = new ProjectJoinedIntegrationEvent
            {
               Company = notification.Company,
               Introduction = notification.Introduction,
               Contributor = notification.contributor

            };
            _capPublisher.Publish("zhengwei.projectapi.projectjoin", @event);
            return Task.CompletedTask;
        }
    }
}
