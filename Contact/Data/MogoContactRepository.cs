using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zhengwei.Contact.Api.Dtos;
using Zhengwei.Contact.Api.Models;

namespace Zhengwei.Contact.Api.Data
{
    public class MogoContactRepository : IContactRepository
    {
        private ContactContext _contactContext;
        public MogoContactRepository(ContactContext contactContext)
        {
            _contactContext = contactContext;
        }
        /// <summary>
        /// 给某一个用户名下联系薄里面加一个联系人，
        /// 如果这个人没有联系薄要新建一个联系薄，再往里面加
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="contact"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task<bool> AddContact(int userId,UserIdentity contact, CancellationToken cancellation)
        {
            long count = _contactContext.ContactBooks.Count(c => c.UserId == userId);
            if(count == 0)
            {
                await _contactContext.ContactBooks.InsertOneAsync(new ContactBook() { UserId = userId });
            }
            var filter = Builders<ContactBook>.Filter.Eq(c => c.UserId, userId);
            var update = Builders<ContactBook>.Update.AddToSet(c => c.Contacts, new Models.Contact
            {
                UserId = contact.UserId,
                Avatar = contact.Avatar,
                Company = contact.Company,
                Name = contact.Name,
                Title = contact.Title
            });
           var result =await _contactContext.ContactBooks.UpdateOneAsync(filter, update, null, cancellation);
            return result.MatchedCount == result.ModifiedCount && result.ModifiedCount == 1;
        }

        public async Task<List<Models.Contact>> GetContactsAsync(int userId, CancellationToken cancellation)
        {
           var contactBook = (await _contactContext.ContactBooks.FindAsync(c => c.UserId == userId)).FirstOrDefault();
            return contactBook == null ? new List<Models.Contact>() : contactBook.Contacts;


        }

        public async Task<bool> TagContactAsync(int userId,int contactId,List<string> tags,CancellationToken token)
        {
            var filter = Builders<ContactBook>.Filter.And(
                Builders<ContactBook>.Filter.Eq(c=>c.UserId, userId),
                Builders<ContactBook>.Filter.Eq("Contacts.UserId",contactId)
                );
            var upadte = Builders<ContactBook>.Update
                         .Set("Contacts.$.Tags", tags);
            var result = await _contactContext.ContactBooks.UpdateOneAsync(filter, upadte, null, token);
            return result.MatchedCount == result.ModifiedCount && result.MatchedCount == 1;
        }

        // 更新所有有该联系人的信息，如果张三个李四通讯录中都有王五，那都要更新
        public async  Task<bool> UpdateContactInfo(BaseUserInfo userInfo, CancellationToken cancellationToken)
        {
            var contactBook = (await _contactContext.ContactBooks.FindAsync(c => c.UserId == userInfo.UserId, null, cancellationToken)).FirstOrDefault(cancellationToken);
            if (contactBook == null) return true;

            var contactIds = contactBook.Contacts.Select(c => c.UserId);

            var filter = Builders<ContactBook>.Filter.And(
                 Builders<ContactBook>.Filter.In(c=>c.UserId,contactIds),
                 Builders<ContactBook>.Filter.ElemMatch(c=>c.Contacts,contact=>contact.UserId == userInfo.UserId)
                );
            var update = Builders<ContactBook>.Update
                .Set("Contacts.$.Name",userInfo.Name)
                .Set("Contacts.$.Avatar", userInfo.Avatar)
                .Set("Contacts.$.Company", userInfo.Company)
                .Set("Contacts.$.Title", userInfo.Title);
            var upadteResult = _contactContext.ContactBooks.UpdateMany(filter, update);
            return upadteResult.MatchedCount == upadteResult.ModifiedCount;

        }
    }
}
