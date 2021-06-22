using GoldContactBookPro.Common;
using GoldContactBookPro.DTO;
using GoldContactBookPro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldContactBookPro.Controllers
{
    [Route("[Controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJWT_TokenGenerator _tokenGenerator;

        public UserController(UserManager<Users> userManager, SignInManager<Users> signInManager, RoleManager<IdentityRole> roleManager, IJWT_TokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO model)
        {
            var exists = await _userManager.FindByEmailAsync(model.Email);

            if (exists != null) return BadRequest("Sorry, this email already exists");

            var newUser = new Users()
            {
                FirstName = model.FirstName,
                Lastname = model.LastName,
                Email = model.Email,
                UserName = model.Email
            };


            if (await _roleManager.FindByNameAsync("Regular") == null)
            {
               await _roleManager.CreateAsync(new IdentityRole("Regular"));
            }

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (!result.Succeeded) return StatusCode(500, "Sorry, try again later");

            var checkRole = await _userManager.AddToRoleAsync(newUser, "Regular");

            if (!checkRole.Succeeded) return StatusCode(500, "Try later");
            
            return Ok("Registration successful");    
        }


        [HttpPost]
        [Route("makeadmin/{id}")]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            var exist = await _userManager.FindByIdAsync(id);

            if (exist == null) return NotFound("User does not exist");

            if (await _roleManager.FindByNameAsync("Admin") == null)
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var attempt = await _userManager.AddToRoleAsync(exist, "Admin");

            if (!attempt.Succeeded)
                return StatusCode(500, "Attempt not successful");

            return Ok("You're now an ADMIN");
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LogIn([FromBody] LogInDTO logInDetail)
        {
            var exist = await _userManager.FindByEmailAsync(logInDetail.Email);

            if (exist == null) return NotFound("Email does not exist.");

            var response = await _signInManager.PasswordSignInAsync(exist, logInDetail.Password, false, false);

            if (!response.Succeeded) return BadRequest("Invalid login details");

            var getToken = await _tokenGenerator.GenerateToken(exist);

            var token = new LogInResponseDTO
            {
                Token = getToken,
            };
            return Ok(token);
        }
    }
}
