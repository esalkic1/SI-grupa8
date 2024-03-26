using System;
using System.Text;
using AutoMapper;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services
{
	public class UserService : IUserService
	{
		private readonly IMapper _mapper;
		private readonly IUserRepository _userRepository;

		public UserService(IUserRepository userRepository, IMapper mapper)
		{
			_userRepository = userRepository;
			_mapper = mapper;
		}
        public async Task<UserDto> AddUser(UserRegisterDto userRegisterDto)
		{
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password);

            User user = new User
            {
                Email = userRegisterDto.Email,
                PhoneNumber = userRegisterDto.PhoneNumber,
                PasswordHash = Encoding.UTF8.GetBytes(passwordHash),
                PasswordSalt = [],
                RoleID = 0
            };
            _userRepository.Add(user);
            await _userRepository.SaveChangesAsync();

            var userDto =  _mapper.Map<UserDto>(user);
            return  userDto;

        }

        public async Task<List<User>> GetAll()
        {
            return await _userRepository.GetAll();
        }

        public async Task RefreshUserToken(int userID, RefreshTokenDto refreshTokenDto)
        {
            var existingUser = await _userRepository.GetById(userID);

            if (existingUser != null)
            {
                // Update the properties of the existing user with the new token information
                existingUser.RefreshToken = refreshTokenDto.RefreshToken;
                existingUser.TokenCreated = refreshTokenDto.TokenCreated.ToUniversalTime();
                existingUser.TokenExpires = refreshTokenDto.TokenExpires.ToUniversalTime(); // Adjust expiration as needed

                // Save changes to the database
                await _userRepository.SaveChangesAsync();

                var userDto = _mapper.Map<UserDto>(existingUser);
                // Return or use the updated userDto as needed
            }
            else
            {
                // Handle the case where the user with the specified ID does not exist
                // This could be logging an error, throwing an exception, or any other appropriate action
            }
            /*var oldUser = await _userRepository.GetById(userID);
            User user = new User
            {

                Email = oldUser.Email,
                PhoneNumber = oldUser.PhoneNumber,
                PasswordHash = oldUser.PasswordHash,
                PasswordSalt = oldUser.PasswordSalt,
                RoleID = oldUser.RoleID,
                RefreshToken = refreshTokenDto.RefreshToken,
                TokenCreated = refreshTokenDto.TokenCreated.ToUniversalTime(),
                //TokenExpires = refreshTokenDto.TokenExpires.ToUniversalTime()
                TokenExpires = DateTime.Now.AddSeconds(20).ToUniversalTime()
            };
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);*/
        }
    }
}
