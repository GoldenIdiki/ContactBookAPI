using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoldContactBookPro.DTO
{
    public class RegisterUserDTO
    {
        [Required]
        [MinLength(1), MaxLength(20)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(1), MaxLength(20)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
