using System;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace DAL.Repositories
{
	public class UserRepository : Repository<User>,  IUserRepository
	{
		private readonly AppDbContext _context;

		public UserRepository(AppDbContext context) : base(context)
		{
			_context = context;
		}

        public void Add(User user)
        {
            _context.Users.Add(user);
            
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

   
    }
}

