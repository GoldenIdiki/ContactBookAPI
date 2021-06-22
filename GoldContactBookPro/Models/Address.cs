using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoldContactBookPro.Models
{
    public class Address
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        [ForeignKey("ContactId")]
        public string ContactId { get; set; }

        //public Contact Contact { get; set; }
    }
}
