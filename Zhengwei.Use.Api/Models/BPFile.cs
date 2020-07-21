using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zhengwei.UserApi.Models
{
    public class BPFile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        //文件名
        public string FileName { get; set; }
        //上传源文件的地址
        public string OriginFilePath { get; set; }
        //格式转化后的地址
        public string FromatFilePath { get; set; }
        public DateTime CreatedTime { get; set; }

    }
}
