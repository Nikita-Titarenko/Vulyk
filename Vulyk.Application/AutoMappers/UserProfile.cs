using AutoMapper;
using Vulyk.Application.DTOs.UserManagement;

namespace Vulyk.Application.AutoMappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Domain.Models.User, UserDto>();
            CreateMap<IEnumerable<Domain.Models.User>, UsersDto>().ForMember(dto => dto.Users, model => model.MapFrom(model => model));
        }
    }
}
