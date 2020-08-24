using BookStore_API.Contracts;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
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

        public UsersController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ILoggerService logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

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
                    var user = await _userManager.FindByNameAsync(username);
                    return Ok(user);
                }
                _logger.LogWarn($"{username} not authenticated");
                return Unauthorized(userDTO);
            }
            catch(Exception ex)
            {
                return InternalError(ex);
            }
        }

        private ObjectResult InternalError(Exception ex)
        {
            string msg = ex.Message + (ex.InnerException != null ? $" - {ex.InnerException.Message}" : "");
            _logger.LogError(msg);
            return StatusCode(500, "Something went wrong. Please contact your Administrator!");
        }
    }
}
