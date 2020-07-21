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
    public class ProjectViewerDomainEventHandler : INotificationHandler<ProjectViewedEvent>
    {
        public ICapPublisher _capPublisher;
        public ProjectViewerDomainEventHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public Task Handle(ProjectViewedEvent notification, CancellationToken cancellationToken)
        {
            var @event = new ProjectViewerIntegrationEvent
            {
                Company = notification.Company,
                Introduction = notification.Introduction,
                Viewer = notification.Viewer

            };
            _capPublisher.Publish("zhengwei.projectapi.projectview", @event);
            return Task.CompletedTask;
        }
    }
}
