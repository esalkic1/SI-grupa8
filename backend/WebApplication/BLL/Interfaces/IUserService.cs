﻿using BLL.DTOs;
using DAL.Entities;
using Google.Authenticator;
using Microsoft.AspNetCore.Http;
using System;
namespace BLL.Interfaces
{
	public interface IUserService
	{
        Task<UserDto> AddUser(UserRegisterDto userRegisterDto);
        Task<List<User>> GetAll();
        Task<SetupCode> SetupCode(User user);
        Task<User> GetUserByPhoneNumber(string phoneNumber);
        Task<User> GetUserByEmail(string email);
        Task RefreshUserToken(int userID, RefreshTokenDto refreshTokenDto);
        Task<User> GetByToken(string token);
        Task<User> UpdateUser(User user);
        Task<(CookieOptions cookiesOption, string refreshToken, object data)> UserLogIn(UserLogIn userRequest);
        Task<object> EnableTwoFactorAuthentication(int userID);
    }
}

