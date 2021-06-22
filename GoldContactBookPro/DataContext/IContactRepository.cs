using GoldContactBookPro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldContactBookPro.DataContext
{
    public interface IContactRepository
    {
        Task<bool> AddContact(Contact contact);
        Task<bool> AddListOfContacts(List<Contact> listOfContacts);
        List<Contact> GetAllContacts();
        Contact GetContactById(string id);
        Contact GetContactByEmail(string email);
        Task<bool> UpdateContact(Contact contact);
        Task<bool> DeleteContact(Contact contact);
        bool IsContactExist(string phoneNumber, string firstName);
        Contact CreateContact(string firstName, string lastName, string email, string phoneNumber, string photoPath, Address address);
        Contact GetContactByQuery(string query);
    }
}
