using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using myStore.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace myStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountApiController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return Ok(new { Message = "User registered successfully." });
                }

                return BadRequest(new { Errors = result.Errors });
            }

            return BadRequest(new { Message = "Invalid model state." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var token = await GenerateJwtToken(user);
                    return Ok(new { Message = "Login successful.", Token = token, Email = user.Email, Id = user.Id });
                }

                return Unauthorized(new { Message = "Invalid login attempt." });
            }

            return BadRequest(new { Message = "Invalid model state." });
        }

        private async Task<string> GenerateJwtToken(IdentityUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim("id", user.Id));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "V29ya2V5aW4hTm93VGhpcyIsVGhlIGtleSBpcyBtYWRlIGZvc3RlciBDdWzXcQ=="));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { Message = "Logout successful." });
        }

        [Authorize]
        [HttpGet("userdata")]
        public IActionResult GetUserData()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            return Ok(new
            {
                Id = userId,
                Email = email
            });
        }

        [Authorize]
        [HttpDelete("deleteAccount")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userId == null)
            {
                return BadRequest(new { Message = "User not found." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "User account not found." });
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                return Ok(new { Message = "Account deleted successfully." });
            }

            return BadRequest(new { Errors = result.Errors });
        }

        //[Authorize(Roles = "Admin")]
        [HttpDelete("deleteAccount/{userId}")]
        public async Task<IActionResult> DeleteUserById(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new { Message = "User ID is required." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "User account not found." });
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { Message = $"User with ID {userId} deleted successfully." });
            }

            return BadRequest(new { Errors = result.Errors });
        }
    }
}
