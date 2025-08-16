using AutoMapper;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Manage.Internal;
using Vulyk.Areas.Identity.Pages.Account;
using Vulyk.DTOs.Account;
using Vulyk.DTOs.Profile;
using Vulyk.DTOs.UserManagement;
using Vulyk.ViewModels;
using Vulyk.ViewModels.User;

namespace Vulyk.AutoMappers
{
    public class UserProfile : Profile
    {
        public UserProfile() {
            CreateMap<RegisterModel.InputModel, RegisterDto>();
            CreateMap<LoginModel.InputModel, LoginDto>();
            CreateMap<Areas.Identity.Pages.Account.Manage.IndexModel.InputModel, UserProfileEditDto>();
            CreateMap<UserProfileEditDto, Areas.Identity.Pages.Account.Manage.IndexModel.InputModel>();
            CreateMap<Areas.Identity.Pages.Account.Manage.ChangePasswordModel.InputModel, ChangePasswordDto>();
            CreateMap<Areas.Identity.Pages.Account.ResetPasswordModel.InputModel, ResetPasswordDto>();
            CreateMap<Areas.Identity.Pages.Account.Manage.SetPasswordModel.InputModel, SetPasswordDto>();
            CreateMap<AuthResultDto, ConfirmTokenDto>();
            CreateMap<UsersDto, UsersViewModel>();
            CreateMap<UserDto, UserViewModel>();
        }
    }
}
