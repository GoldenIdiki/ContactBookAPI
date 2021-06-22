using GoldContactBookPro.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoldContactBookPro.DTO
{
    public class ContactToUpdateDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public Address Address { get; set; }
        public string PhotoPath { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
