using System;
using AutoMapper;
using DAL.Entities;

namespace BLL.Mapper
{
	public class MapperProfile : Profile
	{
		public MapperProfile()
		{
			CreateMap<DAL.Entities.User, DTOs.UserDto>().ReverseMap();
            CreateMap<DAL.Entities.Role, DTOs.RoleDto>().ReverseMap();
            CreateMap<DAL.Entities.RefreshToken, DTOs.RefreshTokenDto>().ReverseMap();
        }
	}
}

