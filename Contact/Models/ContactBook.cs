﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zhengwei.Contact.Api.Models
{
    public class ContactBook
    {
        public ContactBook()
        {
            Contacts = new List<Contact>();
        }
        public int UserId { get; set; }
        public List<Contact> Contacts {get;set;}
    }
}
