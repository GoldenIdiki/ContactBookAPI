using GoldContactBookPro.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldContactBookPro.DataContext
{
    public class ContactRepository : IContactRepository
    {
        private readonly ContactDbContext _db;
        public ContactRepository(ContactDbContext db)
        {
            _db = db;
        }
        public Contact CreateContact(string firstName, string lastName, string email, string phoneNumber, string photoPath, Address address)
        {

            return new Contact
            {
                Firstname = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                PhotoUrl = photoPath,
                Address = address
            };
        }
        public async Task<bool> AddContact(Contact contact)
        {
            bool isSuccess = false;

            await _db.AddAsync(contact);

            int check = await _db.SaveChangesAsync();

            if (check >= 1) isSuccess = true;

            return isSuccess;
        }

        public async Task<bool> AddListOfContacts(List<Contact> listOfContacts)
        {
            bool isSuccess = false;

            await _db.AddRangeAsync(listOfContacts);

            int check = await _db.SaveChangesAsync();

            if (check >= 1) isSuccess = true;

            return isSuccess;
        }

        public async Task<bool> DeleteContact(Contact contact)
        {
            bool isSuccess = false;

            _db.ContactTbl.Remove(contact);

            int check = await _db.SaveChangesAsync();

            if (check >= 1) isSuccess = true;

            return isSuccess;
        }

        public Contact GetContactByEmail(string email)
        {
            Contact contact = _db.ContactTbl.FirstOrDefault(ct => ct.Email == email);

            if (contact == null) throw new Exception("Contact does not exist");

            return contact;
        }

        public Contact GetContactById(string id)
        {
            Contact contact = _db.ContactTbl.FirstOrDefault(ct => ct.Id == id);

            if (contact == null) throw new Exception("Contact does not exist");

            return contact;
        }
        public List<Contact> GetAllContacts()
        {
            return _db.ContactTbl.Select(cont => cont).ToList();
        }

        public Contact GetContactByQuery(string query)
        {
            return _db.ContactTbl.Include(x => x.Address).FirstOrDefault(x => x.Email == query || x.Id == query || x.PhoneNumber == query);
        }
        public async Task<bool> UpdateContact(Contact contact)
        {
            bool isSuccess = false;

            int check = 0;

            var contactToUpdate = _db.ContactTbl.FirstOrDefault(ct => ct.Id == contact.Id);

            _db.Entry(contactToUpdate).CurrentValues.SetValues(contact);

            check = await _db.SaveChangesAsync();

            if (check >= 1) isSuccess = true;

            return isSuccess;
        }
        public bool IsContactExist(string phoneNumber, string firstName)
        {
            return _db.ContactTbl.Any(contact => contact.Firstname == firstName && contact.PhoneNumber == phoneNumber);
        }
    }
}
