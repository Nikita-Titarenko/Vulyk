using AutoMapper;
using Vulyk.DTOs;
using Vulyk.ViewModels;

namespace Vulyk.AutoMappers
{
    public class UserProfile : Profile
    {
        public UserProfile() {
            CreateMap<FullNameViewModel, FullNameDto>();
            CreateMap<UserEditDto, EditProfileViewModel>();
            CreateMap<EditProfileViewModel, UserEditDto>();
            CreateMap<EmailViewModel, EmailConfirmDto>();
            CreateMap<RegisterViewModel, RegistrationDto>();
        }
    }
}
