using System;
namespace DAL.Entities
{
	public class User
	{
		public int UserID { get; set; }
		public string Email { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Surname { get; set; } = string.Empty;
		public byte[] PasswordHash { get; set; }
		public byte[] PasswordSalt { get; set; }
		public int RoleID { get; set; }

		public Role Role { get; set; }

		public string RefreshToken { get; set; } = string.Empty;
		public DateTime TokenCreated { get; set; }
		public DateTime TokenExpires { get; set; }
		
	}
}

