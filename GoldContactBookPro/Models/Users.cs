using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoldContactBookPro.Models
{
    public class Users : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        public string Lastname { get; set; }
    }
}
