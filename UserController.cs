using Microsoft.AspNetCore.Mvc;
using E_Commerce.Models;
using E_Commerce.DTOs;
using E_Commerce.Services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        UserServices _userServices;
        E_CommerceDB _context;
        IConfiguration _config;
        public UserController(UserServices userServices, E_CommerceDB context, IConfiguration config)
        {
            _userServices = userServices;
            _context = context;
            _config = config;
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ActionResult<AppUser>> Register([FromBody] UserDTO userDTO)
        {

            if (userDTO == null)
            {
                return BadRequest("User object is null");
            }
            var user = new AppUser
            {
                Name = userDTO.Name,
                Email = userDTO.Email,
                Password = userDTO.Password,
                IsAdmin = false

            };
            var name = await _userServices.UserExists(userDTO.Name);
            if (name == false)
            {
                return BadRequest("user already exists");


            }
            var newuser = await _userServices.AddUser(userDTO);

            return Ok(new { message = $"user created  successfully with id = {newuser.Id}" });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> login([FromQuery] string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return Unauthorized("Invalid user data");
            }

            var user = await _userServices.GetUserByUsername(username);

            if (user != null)
            {
                if (_userServices.VerifyPassword(username, password))
                {

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,user.Name),
                        new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
                    };
                    if (user.IsAdmin)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "User"));
                    }

                    SigningCredentials creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:secret"])), SecurityAlgorithms.HmacSha256);
                    JwtSecurityToken token = new JwtSecurityToken(
                                            issuer: _config["JWT:validIssuer"],
                                            audience: _config["JWT:validAudience"],
                                            claims: claims,
                                            expires: DateTime.Now.AddMinutes(30),
                                            signingCredentials: creds
                                            );
                    return Ok(new { msg = "Valid User, Logged in successfully", token = new JwtSecurityTokenHandler().WriteToken(token), expiration = token.ValidTo });
                }
                else
                {
                    return Unauthorized(new { message = "Password is incorrect" });
                }
            }
            else
            {
                return NotFound(new { message = "Username is incorrect" });
            }
        }

        [HttpGet("getAllUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<AppUser>>> getAllUsers()
        {
            var users = await _userServices.GettAllUsers();

            if (users == null || !users.Any())
            {
                return NotFound("No users found");
            }
            return Ok(users);
        }


    }
}
