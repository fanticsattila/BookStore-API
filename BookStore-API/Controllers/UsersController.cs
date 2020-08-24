using BookStore_API.Contracts;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;

        public UsersController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ILoggerService logger, IConfiguration config)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _config = config;
        }

        public object Configuration { get; private set; }

        /// <summary>
        /// User login endpoint
        /// </summary>
        /// <param name="userDTO">User's login data</param>
        /// <returns>Authorize or not</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            try
            {
                string username = userDTO.UserName;
                string password = userDTO.Password;

                _logger.LogInfo($"Login attempt from user {username}");


                var result = await _signInManager.PasswordSignInAsync(username, password, false, false);
                if (result != null && result.Succeeded)
                {
                    _logger.LogInfo($"{username} successfully authenticated");
                    IdentityUser user = await _userManager.FindByNameAsync(username);
                    string tokenString = await GenerateJSonWebToken(user);
                    return Ok(new { token = tokenString });
                }
                _logger.LogWarn($"{username} not authenticated");
                return Unauthorized(userDTO);
            }
            catch(Exception ex)
            {
                return InternalError(ex);
            }
        }

        private async Task<string> GenerateJSonWebToken(IdentityUser user)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r)));
            JwtSecurityToken token = new JwtSecurityToken(
                _config["Jwt:Issuer"], 
                _config["Jwt:Issuer"], 
                claims, 
                null, 
                expires:DateTime.Now.AddMinutes(5),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ObjectResult InternalError(Exception ex)
        {
            string msg = ex.Message + (ex.InnerException != null ? $" - {ex.InnerException.Message}" : "");
            _logger.LogError(msg);
            return StatusCode(500, "Something went wrong. Please contact your Administrator!");
        }
    }
}
