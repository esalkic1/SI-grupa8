using BLL.DTOs;
using DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using DAL.Repositories;
using DAL.Interfaces;
using BLL.Interfaces;
using BLL.Services;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user =new User();
        private readonly IConfiguration _config;

        IUserService _userService;
         public AuthController(IConfiguration config,IUserService userService)
         {
             _userService = userService;
            _config=config;
         }
        

       [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(UserRegisterDto userRegisterDto)
            
        {
            //Check if input contains at least an email or a phone number

            if (userRegisterDto.Email.IsNullOrEmpty() && userRegisterDto.PhoneNumber.IsNullOrEmpty())
                return BadRequest("Cannot register without at least an email or a phone number!");
            var userDto = await _userService.AddUser(userRegisterDto);
            return Ok(userDto);
        }


        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserRegisterDto request)
        {
            //Check if input contains at least an email or a phone number
            if (request.Email.IsNullOrEmpty() && request.PhoneNumber.IsNullOrEmpty())
                return BadRequest("Cannot login without at least an email or a phone number!");

            List<User> users = await _userService.GetAll();
            User user = new User();

            //Look for a user by email
            if (!request.Email.IsNullOrEmpty()) {
                user = users.FirstOrDefault(u => u.Email == request.Email);
                if (user == null) { return BadRequest("User not found"); }
            }
            //Look for a user by phone number
            else if (!request.PhoneNumber.IsNullOrEmpty())
            {
                user = users.FirstOrDefault(u => u.PhoneNumber == request.PhoneNumber);
                if (user == null) { return BadRequest("User not found"); }
            }

            string hash = Encoding.UTF8.GetString(user.PasswordHash);

            if (!BCrypt.Net.BCrypt.Verify(request.Password, Encoding.UTF8.GetString( user.PasswordHash)))
            {
                return BadRequest("Wrong password.");
            }

            string token = CreateToken(user);
            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken);
            RefreshTokenDto refresh = new RefreshTokenDto()
            {
                RefreshToken = refreshToken.Token,
                TokenCreated = refreshToken.Created,
                TokenExpires = refreshToken.Expires
            };
            await _userService.RefreshUserToken(user.UserID, refresh);

            return Ok(token);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            List<User> users = await _userService.GetAll();
            User user = users.FirstOrDefault(u => u.RefreshToken == refreshToken);

            if (user == null)
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            else if (user.TokenExpires < DateTime.Now.ToUniversalTime())
            {
                return Unauthorized("Token expired.");
            }

            string token = CreateToken(user);
            var newToken = GenerateRefreshToken();
            SetRefreshToken(newToken);
            RefreshTokenDto refresh = new RefreshTokenDto()
            {
                RefreshToken = newToken.Token,
                TokenCreated = newToken.Created,
                TokenExpires = newToken.Expires
            };
            await _userService.RefreshUserToken(user.UserID, refresh);

            return Ok(token);

        }

        private RefreshToken GenerateRefreshToken() 
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddMinutes(30),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken newRefreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };

            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims:claims,
                expires:DateTime.Now.AddMinutes(30),
                signingCredentials:credentials
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

    }
}
