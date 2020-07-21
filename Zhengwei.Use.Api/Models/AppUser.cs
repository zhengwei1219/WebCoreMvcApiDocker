using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zhengwei.UserApi.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }

        public string Title { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public string Gender { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }
        public string Tel { get; set; }
        public string ProvinceId { get; set; }
        public string CityId { get; set; }
        public string City { get; set; }
        public string NameCard { get; set; }
        public List<UserProperty> Properties { get; set; }
    }
}
