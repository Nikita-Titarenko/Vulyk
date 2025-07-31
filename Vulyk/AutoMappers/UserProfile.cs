using AutoMapper;
using Vulyk.DTOs;
using Vulyk.ViewModels;

namespace Vulyk.AutoMappers
{
    public class UserProfile : Profile
    {
        public UserProfile() {
            CreateMap<NameAndPasswordInputViewModel, NameAndPasswordInputDto>();
        }
        
    }
}
