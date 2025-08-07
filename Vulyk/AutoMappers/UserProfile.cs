using AutoMapper;
using Vulyk.DTOs;
using Vulyk.ViewModels;

namespace Vulyk.AutoMappers
{
    public class UserProfile : Profile
    {
        public UserProfile() {
            CreateMap<FullNameViewModel, FullNameDto>();
            CreateMap<UserProfileEditDto, EditProfileViewModel>();
            CreateMap<EditProfileViewModel, UserProfileEditDto>();
            CreateMap<EmailViewModel, EmailConfirmDto>();
            CreateMap<RegisterViewModel, RegistrationDto>();
            CreateMap<EditPasswordViewModel, EditPasswordByCurrentPasswordDto>();
            CreateMap<ResetPasswordViewModel, ResetPasswordDto>();
        }
    }
}
