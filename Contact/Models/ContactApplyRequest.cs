using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zhengwei.Contact.Api.Models
{
    public class ContactApplyRequest
    {
        public int Id { get; set; }
        //被添加的人信息
        public int UserId { get; set; }

        public string Name { get; set; }
        public string Company { get; set; }
        public string Title { get; set; }
        //用户头像
        public string Avatar { get; set; }
        //申请人Id
        public int ApplierId {get;set;}
        //是否通过
        public int Approvaled { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime HandleTime { get; set; }
        public DateTime CreatedTime { get; set; }

    }
}
