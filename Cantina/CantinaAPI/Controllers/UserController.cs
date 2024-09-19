using CantinaAPI.Auth;
using CantinaAPI.Models;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace CantinaAPI.Controllers
{
    [ApiController]
    [Route("/user/")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuth _auth;
        public AccountController(UserManager<User> userManager, IAuth auth)
        {
            _userManager = userManager;
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    Location = model.Location,
                    PhoneNumber = model.PhoneNumber
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    return Ok(new { message = "User registered successfully" });
                }
                return BadRequest(result.Errors);
            }
            return BadRequest("Invalid data");
        }

        [Authorize]
        [HttpGet("getRoles")]
        public IActionResult GetCurrentUserRoles()
        {
            var roles = User.Claims
                            .Where(c => c.Type == ClaimTypes.Role)
                            .Select(c => c.Value)
                            .ToList();

            return Ok(roles);
        }

        [HttpPost("addRole")]
        public async Task<IActionResult> AddRoleToUser([Required] string role, [Required][FromQuery] string userEmail ,[Required][FromBody] string code)
        {
            if(code != "1616")
            {
                return BadRequest("Invalid code");
            }
            var email = userEmail;
            if (email == null)
            {
                return BadRequest("Email claim not found");
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.AddToRoleAsync(user, role);

            if (result.Succeeded)
            {
                return Ok("Role added successfully");
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.email);
            if (user == null)
            {
                return BadRequest("Invalid email or password");
            }

            var result = await _userManager.CheckPasswordAsync(user, model.password);
            if (result)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var token = _auth.GenerateJWTToken(user, roles);
                return Ok(token);
            }

            return BadRequest("Invalid email or password");
        }

    }
}
