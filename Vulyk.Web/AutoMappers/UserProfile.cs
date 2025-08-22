using AutoMapper;
using Vulyk.Application.DTOs.Account;
using Vulyk.Application.DTOs.Profile;
using Vulyk.Application.DTOs.UserManagement;
using Vulyk.Web.ApiModels.Requests;
using Vulyk.Web.ApiModels.Responds;
using Vulyk.Web.Areas.Identity.Pages.Account;
using Vulyk.Web.ViewModels.UserManagement;

namespace Vulyk.Web.AutoMappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
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

            CreateMap<UserProfileEditDto, ProfileResponseModel>();
            CreateMap<LoginRequestModel, LoginDto>();
            CreateMap<RegisterRequestModel, RegisterDto>();
            CreateMap<RegisterDto, AuthResponseModel>();
        }
    }
}
