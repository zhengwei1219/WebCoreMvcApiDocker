using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhengwei.Contact.Api.Models;
using MongoDB.Driver;
using System.Threading;

namespace Zhengwei.Contact.Api.Data
{
    public class MogoContactApplyRequestRepository : IContactApplyRequestRepository
    {
        private readonly ContactContext _contactContext;
        public MogoContactApplyRequestRepository(ContactContext contactContext, CancellationToken cancellationToken)
        {
            _contactContext = contactContext;
        }

        /// <summary>
        /// //查入一条请求信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> AddRequestAsync(ContactApplyRequest request,CancellationToken cancellationToken)
        {
            var filter = Builders<ContactApplyRequest>.Filter.Where(r => r.UserId == request.UserId && r.ApplierId == request.ApplierId);
            if((await _contactContext.ContactApplyRequests.CountAsync(filter))>0)
            {
                var update = Builders<ContactApplyRequest>.Update.Set(r => r.CreatedTime, DateTime.Now);
               var result  = await _contactContext.ContactApplyRequests.UpdateOneAsync(filter, update, null, cancellationToken);
                return result.MatchedCount == result.ModifiedCount && result.MatchedCount == 1;

            }

            await _contactContext.ContactApplyRequests.InsertOneAsync(request, null, cancellationToken);
            return true;
        }

        /// <summary>
        ///  //通过一条申请信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="applierId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> ApprovalAsync(int userId,int applierId, CancellationToken cancellationToken)
        {
            var filter = Builders<ContactApplyRequest>.Filter.Where(r => r.UserId == userId && r.ApplierId == applierId);

            var update = Builders<ContactApplyRequest>.Update
            .Set(r => r.Approvaled,1)
            .Set(r => r.HandleTime, DateTime.Now);
            
                
                var result = await _contactContext.ContactApplyRequests.UpdateOneAsync(filter, update, null, cancellationToken);
                return result.MatchedCount == result.ModifiedCount && result.MatchedCount == 1;

            
        }

        /// <summary>
        /// 获取某个用户的所有请求加好友的记录。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<ContactApplyRequest>> GetRequestListAsync(int userId,CancellationToken cancellationToken)
        {
            return (await _contactContext.ContactApplyRequests.FindAsync(a => a.UserId == userId)).ToList(cancellationToken);
        }
    }
}
