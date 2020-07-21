using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zhengwei.Contact.Api.Data;
using Zhengwei.Contact.Api.Dtos;
using Zhengwei.Contact.Api.IntegrationEvents.Events;

namespace Zhengwei.Contact.Api.IntegrationEvents.EventHandling
{
    public class UserProfileChangedEventHandler:ICapSubscribe
    {
        private IContactRepository _contactRepository;

        public UserProfileChangedEventHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        [CapSubscribe("zhengwei.userapi.userchanged")]
        public async Task UpdateContactInfo(UserProfileChangedEvent @event)
        {
            var token = new CancellationToken();
            await _contactRepository.UpdateContactInfo(new BaseUserInfo
            {
                Avatar= @event.Avatar,
                Name = @event.Name,
                Company = @event.Company,
                Title = @event.Title
            },token);
        }
    }
}
