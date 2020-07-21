using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zhengwei.Contact.Api.Dtos;

namespace Zhengwei.Contact.Api.Data
{
   public interface IContactRepository
    {
        /// <summary>
        /// 更新联系人信息
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> UpdateContactInfo(BaseUserInfo userInfo, CancellationToken cancellationToken);
        /// <summary>
        /// 添加联系人信息
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        Task<bool> AddContact(int contactId,UserIdentity contact, CancellationToken cancellation);
        /// <summary>
        /// 获取联系人列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<Models.Contact>> GetContactsAsync(int userId, CancellationToken cancellation);
        /// <summary>
        /// 更新好友标签
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        Task<bool> TagContactAsync(int userId, int contactIds,List<string> tags, CancellationToken token);

    }
}
