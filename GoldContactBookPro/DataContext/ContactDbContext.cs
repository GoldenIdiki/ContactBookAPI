using GoldContactBookPro.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldContactBookPro.DataContext
{
    public class ContactDbContext : IdentityDbContext<Users>
    {
        private readonly DbContextOptions<ContactDbContext> _options;

        public ContactDbContext(DbContextOptions<ContactDbContext> options) : base(options)
        {
            _options = options;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<Users> UsersTbl { get; set; }
        public DbSet<Contact> ContactTbl { get; set; }
        public DbSet<Address> AddressTbl { get; set; }

    }
}
