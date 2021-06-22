using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using GoldContactBookPro.DataContext;
using GoldContactBookPro.DTO;
using GoldContactBookPro.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldContactBookPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ContactController : ControllerBase
    {
        private readonly IContactRepository _contactRepo;
        private readonly IConfiguration _configuration;
        private Cloudinary _cloudinary;
        public ContactController(IContactRepository contactRepo, IConfiguration configuration)
        {
            _contactRepo = contactRepo;
            _configuration = configuration;
            Account account = new Account
            {
                Cloud = _configuration.GetSection("CloudinarySettings:CloudName").Value,
                ApiKey = _configuration.GetSection("CloudinarySettings:ApiKey").Value,
                ApiSecret = _configuration.GetSection("CloudinarySettings:ApiSecret").Value
            };
            _cloudinary = new Cloudinary(account);
        }


        [HttpPost("add-new")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddContact([FromBody] ContactToAddDTO contact)
        {
            var contactExists = _contactRepo.IsContactExist(contact.PhoneNumber, contact.FirstName);

            if (contactExists) return BadRequest("Contact already exists.");

            var newContact = _contactRepo.CreateContact(contact.FirstName, contact.LastName, contact.Email, contact.PhoneNumber, contact.PhotoPath, contact.Address);

            bool addContactSuccess = await _contactRepo.AddContact(newContact);

            if (addContactSuccess)
            {
                return Ok("Successfully added");
            }
            return StatusCode(500, "Not successfully added");

        }


        [HttpGet("{query}")]
        [Authorize(Roles = ("Admin, Regular"))]
        public ActionResult GetContactbyEmailOrId(string query)
        {
            Contact fetchContact = _contactRepo.GetContactByQuery(query);

            if (fetchContact == null) return BadRequest("Contact does not exist");

            ContactToReturnDTO contact = new ContactToReturnDTO
            {
                Id = fetchContact.Id,
                FirstName = fetchContact.Firstname,
                LastName = fetchContact.LastName,
                Email = fetchContact.Email,
                Address = fetchContact.Address,
                PhoneNumber = fetchContact.PhoneNumber,
                PhotoPath = fetchContact.PhotoUrl
            };
            return Ok(contact);
        }


        [HttpPut]
        [Route("update/{id}")]
        [Authorize(Roles = ("Admin"))]
        public async Task<ActionResult> UpdateContact(string id, [FromBody] ContactToUpdateDTO contactUpdate)
        {
            Contact fetchContact = _contactRepo.GetContactByQuery(id);
            if (fetchContact == null) return BadRequest("Contact does not exist");

            fetchContact.Firstname = contactUpdate.FirstName != null ? contactUpdate.FirstName : fetchContact.Firstname;
            fetchContact.LastName = contactUpdate.LastName != null ? contactUpdate.LastName : fetchContact.LastName;
            fetchContact.PhoneNumber = contactUpdate.PhoneNumber != null ? contactUpdate.PhoneNumber : fetchContact.PhoneNumber;
            fetchContact.PhotoUrl = contactUpdate.PhotoPath != null ? contactUpdate.PhotoPath : fetchContact.PhotoUrl;
            fetchContact.Email = contactUpdate.Email != null ? contactUpdate.Email : fetchContact.PhotoUrl;
            fetchContact.Address = contactUpdate.Address != null ? contactUpdate.Address : fetchContact.Address;

            var isUpdated = await _contactRepo.UpdateContact(fetchContact);
            if (!isUpdated) return StatusCode(500, "Not successfully added.");

            Contact getUpdatedContact = _contactRepo.GetContactByQuery(fetchContact.Id);

            if (getUpdatedContact == null) return StatusCode(500, "Try again Later");

            var returnUpdatedContact = new ContactToReturnDTO
            {
                FirstName = getUpdatedContact.Firstname,
                LastName = getUpdatedContact.LastName,
                Email = getUpdatedContact.Email,
                Address = getUpdatedContact.Address,
                PhotoPath = getUpdatedContact.PhotoUrl,
                PhoneNumber = getUpdatedContact.PhoneNumber
            };
            return Ok(returnUpdatedContact);
        }
        [HttpDelete]
        [Route("delete/{id}")]
        [Authorize(Roles = ("Admin"))]
        public async Task<ActionResult> DeleteContactById(string id)
        {
            Contact fetchContact = _contactRepo.GetContactByQuery(id);
            if (fetchContact == null) return BadRequest("Contact does not exist");
            bool isDeleted = await _contactRepo.DeleteContact(fetchContact);
            if (isDeleted == false) return StatusCode(500, "Sorry, try again later");
            return Ok("Contact deleted");
        }

        [HttpPatch]
        [Route("photo/{id}")]
        [Authorize(Roles = ("Admin, Regular"))]
        public async Task<ActionResult> AddPatchPhoto(string id, [FromForm] UpdatePhotoDTO photoUpdateDto)
        {
            var contactToUpdate = _contactRepo.GetContactByQuery(id);
            if (contactToUpdate == null)
            {
                return NotFound("Contact Not Found!");
            }
            var file = photoUpdateDto.PhotoUrl;
            if (file.Length <= 0) return BadRequest("Invalid File Size");
            var imageUploadResult = new ImageUploadResult();
            using (var fs = file.OpenReadStream())
            {
                var imageUploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, fs),
                    Transformation = new Transformation().Width(300).Height(300).Crop("fill").Gravity("face")
                };
                imageUploadResult = await _cloudinary.UploadAsync(imageUploadParams);
            }
            var publicId = imageUploadResult.PublicId;
            var Url = imageUploadResult.Url.ToString();
            contactToUpdate.PhotoUrl = Url;
            var photoIsUpdatd = await _contactRepo.UpdateContact(contactToUpdate);
            if (!photoIsUpdatd)
            {
                return StatusCode(500, "Something went wrong, try again");
            }
            return Ok($"Photo Path Successfully Updated {publicId}");
        }

        [HttpGet]
        [Route("search")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllContactbySearch([FromQuery] PagingDTO model)
        {
            var contacts = _contactRepo.GetAllContacts().ToList();

            if (contacts.Count <= 0) return NotFound("No Contacts Available in your phone book");
            
            if (!string.IsNullOrEmpty(model.QuerySearch))
            {
                contacts = contacts.Where(x => x.Firstname.ToLower().Contains(model.QuerySearch.ToLower())
                || x.PhoneNumber.Contains(model.QuerySearch)).ToList();
            }
            var count = contacts.Count();
            var currentPage = model.PageNumber;
            var pageSize = model.PageSize;
            var totalCount = count;
            var totalPages = (int)Math.Ceiling(count / (double)pageSize);
            var items = contacts.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            var previousPage = currentPage > 1 ? "Yes" : "No";
            var nextPage = currentPage < totalPages ? "Yes" : "No";
            var pagination = new
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = currentPage,
                TotalPages = totalPages,
                previousPage,
                nextPage,
                QuerySearch = string.IsNullOrEmpty(model.QuerySearch) ? "No Paramater Passed" : model.QuerySearch
            };
            HttpContext.Response.Headers.Add("Pagin-Header", JsonConvert.SerializeObject(pagination));
            return Ok(items);
        }
    }
}
