﻿using System;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
	{
		private readonly AppDbContext _context;

		public UserRepository(AppDbContext context) : base(context)
		{
			_context = context;
		}

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

		public async Task<User> FindByEmail(string email)
		{
			return await _context.Users.Include(u => u.Role).FirstAsync(x => x.Email == email);
		}

        public async Task<User> FindByPhoneNumber(string phoneNumber)
        {
			return await _context.Users.FirstAsync(x => x.PhoneNumber == phoneNumber);
        }

        public async Task<User> GetByToken(string token)
        {
            return await _context.Users.FirstAsync(x => x.RefreshToken == token);
        }

        public async Task<List<User>> GetAllByCompanyId(int companyID)
        {
            return await _context.Users.Where(x => x.CompanyID == companyID).ToListAsync();
        }

        public async Task<List<User>> GetAllByRole(string role)
        {
            return await _context.Users.Where(x => x.Role.RoleName == role).ToListAsync();
        }
    }
}

