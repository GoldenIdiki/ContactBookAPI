using GoldContactBookPro.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoldContactBookPro.DTO
{
    public class ContactToAddDTO
    {
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        public Address Address { get; set; }
        public string PhotoPath { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
