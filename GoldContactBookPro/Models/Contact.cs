using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoldContactBookPro.Models
{
    public class Contact
    {
        [Key]//, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Firstname { get; set; }
        public string LastName { get; set; }
        public string PhotoUrl { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public Address Address { get; set; }
    }
}
