﻿using BLL.DTOs;
using BLL.Interfaces;
using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICompanyService _companyService;
        private readonly IDeviceService _deviceService;

        public AdminController(IConfiguration config, IUserService userService, ICompanyService companyService, IDeviceService deviceService)
        {
            _userService = userService;
            _deviceService = deviceService;
            _companyService = companyService;
        }

        [HttpPost("get-company-users")]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers(AdminCRUDUserDto request)
        {
            var admin=new User();
            if (!request.adminEmail.IsNullOrEmpty())
            {
                admin = await _userService.GetUserByEmail(request.adminEmail);
                if (admin == null || admin.Role.RoleName!="Admin") { return BadRequest("Admin not found"); }
            }

            var company = _companyService.GetCompanyByID(admin.UserID).Result;
            if(company == null) { return BadRequest("Company not found"); }
            var users = await _userService.GetAllByCompanyId(company.CompanyID);
            return users;
        }

        [HttpPost("remove-user")]
        public async Task<ActionResult> RemoveUser(AdminCRUDUserDto request)
        {

            var admin = new User();
            if (!request.adminEmail.IsNullOrEmpty())
            {
                admin = await _userService.GetUserByEmail(request.adminEmail);
                if (admin == null || admin.Role.RoleName != "Admin") { return BadRequest("Admin not found"); }
            }
            var company = _companyService.GetCompanyByID(admin.UserID).Result;
            if (company == null) { return BadRequest("Company not found"); }
            var users = await _userService.GetAllByCompanyId(company.CompanyID);
            var userDto = users.FirstOrDefault(u => u.Email == request.Email && u.PhoneNumber == request.PhoneNumber);
            var user = _userService.GetUserByEmail(userDto.Email).Result;
            _userService.RemoveUser(user);
            return Ok("User successfully removed.");
        }

        [HttpPost("add-user")]
        public async Task<ActionResult<UserDto>> AddUser(AdminCRUDUserDto request)

        {
            var admin = new User();
            if (!request.adminEmail.IsNullOrEmpty())
            {
                admin = await _userService.GetUserByEmail(request.adminEmail);
                if (admin == null || admin.Role.RoleName != "Admin") { return BadRequest("Admin not found"); }
            }
            var company = _companyService.GetCompanyByID(admin.UserID).Result;
            if (company == null) { return BadRequest("Company not found"); }
            if (request.Email.IsNullOrEmpty() && request.PhoneNumber.IsNullOrEmpty())
                return BadRequest("Cannot add auser without an email or a phone number!");
            var user = new UserRegisterDto { Email = request.Email, Name = request.Name, Surname = request.Surname, Password = request.Password, CompanyID = company.CompanyID };
            var userDto = await _userService.AddUser(user);
            return Ok(userDto);
        }

        [HttpPost("update-user")]
        public async Task<ActionResult> UpdateUser(AdminCRUDUserDto request)
        {
            var admin = new User();
            if (!request.adminEmail.IsNullOrEmpty())
            {
                admin = await _userService.GetUserByEmail(request.adminEmail);
                if (admin == null || admin.Role.RoleName != "Admin") { return BadRequest("Admin not found"); }
            }
            var company = _companyService.GetCompanyByID(admin.UserID).Result;
            if (company == null) { return BadRequest("Company not found"); }
            var users = await _userService.GetAllByCompanyId(company.CompanyID);
            var userDto = users.FirstOrDefault(u => u.Email == request.Email && u.PhoneNumber == request.PhoneNumber);
            var user = _userService.GetUserByEmail(userDto.Email).Result;
            if (request.Name!=null) user.Name=request.Name;
            if (request.Surname!=null) user.Surname=request.Surname;
            if (request.Email!=null) user.Email=request.Email;
            if (request.PhoneNumber != null) user.Email = request.PhoneNumber;
            if (request.Password != null) {
                user.PasswordHash = Encoding.UTF8.GetBytes(BCrypt.Net.BCrypt.HashPassword(request.Password));
                user.PasswordSalt = [];
            }
            await _userService.UpdateUser(user);
            return Ok("User successfully updated.");
        }

        [HttpPost("get-company-devices")]
        public async Task<ActionResult<List<DeviceDto>>> GetAllDevices(AdminCRUDDeviceDto request)
        {
            var admin = new User();
            if (!request.adminEmail.IsNullOrEmpty())
            {
                admin = await _userService.GetUserByEmail(request.adminEmail);
                if (admin == null || admin.Role.RoleName != "Admin") { return BadRequest("Admin not found"); }
            }

            var company = _companyService.GetCompanyByID(admin.UserID).Result;
            if (company == null) { return BadRequest("Company not found"); }
            var devices = await _deviceService.GetAllByCompanyId(company.CompanyID);
            return devices;
        }

        [HttpPost("remove-device")]
        public async Task<ActionResult> RemoveDevice(AdminCRUDDeviceDto request)
        {

            var admin = new User();
            if (!request.adminEmail.IsNullOrEmpty())
            {
                admin = await _userService.GetUserByEmail(request.adminEmail);
                if (admin == null || admin.Role.RoleName != "Admin") { return BadRequest("Admin not found"); }
            }
            var company = _companyService.GetCompanyByID(admin.UserID).Result;
            if (company == null) { return BadRequest("Company not found"); }
            var devices = await _deviceService.GetAllByCompanyId(company.CompanyID);
            var deviceDto = devices.FirstOrDefault(u => u.Reference==request.Reference);
            _deviceService.RemoveDevice(deviceDto);
            return Ok("Device successfully removed.");
        }

        [HttpPost("add-device")]
        public async Task<ActionResult> AddDevice(AdminCRUDDeviceDto request)
        {

            var admin = new User();
            if (!request.adminEmail.IsNullOrEmpty())
            {
                admin = await _userService.GetUserByEmail(request.adminEmail);
                if (admin == null || admin.Role.RoleName != "Admin") { return BadRequest("Admin not found"); }
            }
            var company = _companyService.GetCompanyByID(admin.UserID).Result;
            if (company == null) { return BadRequest("Company not found"); }
            var devices = await _deviceService.GetAllByCompanyId(company.CompanyID);
            var deviceDto = devices.FirstOrDefault(u => u.Reference == request.Reference);
            _deviceService.AddDevice(deviceDto);
            return Ok("Device successfully removed.");
        }

        [HttpPost("update-device")]
        public async Task<ActionResult> UpdateDevice(AdminCRUDDeviceDto request)
        {

            var admin = new User();
            if (!request.adminEmail.IsNullOrEmpty())
            {
                admin = await _userService.GetUserByEmail(request.adminEmail);
                if (admin == null || admin.Role.RoleName != "Admin") { return BadRequest("Admin not found"); }
            }
            var company = _companyService.GetCompanyByID(admin.UserID).Result;
            if (company == null) { return BadRequest("Company not found"); }
            var devices = await _deviceService.GetAllByCompanyId(company.CompanyID);
            var deviceDto = devices.FirstOrDefault(u => u.Reference == request.Reference);
            if (request.DeviceName != null) deviceDto.DeviceName = request.DeviceName;
            if (request.Reference != null) deviceDto.Reference = request.Reference;
            _deviceService.UpdateDevice(deviceDto);
            return Ok("Device successfully updated.");
        }
    }
}