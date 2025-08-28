using System.Security.Claims;
using System.Text;
using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Vulyk.Application.DTOs.Account;
using Vulyk.Application.DTOs.Chat;
using Vulyk.Application.DTOs.Profile;
using Vulyk.Application.DTOs.UserManagement;
using Vulyk.Application.Repositories;
using Vulyk.Application.Services.User;
using Vulyk.Infrastructure.Models;

namespace Vulyk.Infrastructure.Services.User
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly ILogger<UserService> _logger;

        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            ILogger<UserService> logger, IUserRepository userRepository, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        ///  <summary>
        ///  Register new user
        ///  </summary>
        ///  <param name="dto">Data for register new user</param>
        ///  <returns>
        ///  <see cref="AuthResultDto"/> with userId and verifiaction code if
        ///  registration success or error information if it fails
        /// </returns>
        public async Task<Result<AuthResultDto>> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email, FullName = dto.FullName };

            var result = await _userManager.CreateAsync(user, dto.Password);

            string userId;

            if (!result.Succeeded)
            {
                if (!result.Errors.Any(e => e.Code == "DuplicateUserName"))
                {
                    _logger.LogWarning("Failed to register: invalid request");
                    return Result.Fail(result.Errors.Select(e => new Error(e.Description).WithMetadata("Code", e.Code)));
                }

                user = await _userManager.FindByEmailAsync(dto.Email);

                if (user!.EmailConfirmed)
                {
                    _logger.LogWarning("Failed to register: User with email already exist");
                    return Result.Fail(result.Errors.Select(e => new Error(e.Description).WithMetadata("Code", e.Code)));
                }

                if (!await _userManager.CheckPasswordAsync(user, dto.Password))
                {
                    _logger.LogWarning("Failed to register: password incorrect");
                    return Result.Fail(result.Errors.Select(e => new Error(e.Description).WithMetadata("Code", e.Code)));
                }

                userId = user.Id;
            }
            else
            {
                userId = await _userManager.GetUserIdAsync(user);
            }

            var confirmTokenDto = await GenerateCurrentEmailConfirmationToken(user);

            return Result.Ok(new AuthResultDto { UserId = userId, Code = confirmTokenDto.Value.Code });
        }

        ///  <summary>
        ///  Login for already existing user
        ///  </summary>
        ///  <param name="dto">Data for user login</param>
        ///  <returns>
        ///  <see cref="AuthResultDto"/> containing:
        ///  <list type="bullet">
        ///  <item>UserId and email confirmation token if the email is not confirmed</item>
        ///  <item>UserId and reset confirmation token if the password is not exist</item>
        ///  <item>Error information if it fails</item>
        /// </list>
        /// </returns>
        public async Task<Result<AuthResultDto>> LoginAsync(LoginDto dto)
        {
            var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, dto.RememberMe, false);

            if (result.Succeeded && !dto.UserIdNeed)
            {
                return Result.Ok(new AuthResultDto { EmailNotConfirmed = false });
            }
            ApplicationUser? user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                _logger.LogWarning("Failed to login: User not found");
                return Result.Fail(new Error("Login failed").WithMetadata("Code", "LoginFailed"));
            }

            if (result.Succeeded)
            {
                return Result.Ok(new AuthResultDto { EmailNotConfirmed = false, UserId = user.Id });
            }

            bool isPasswordCorrect = false;
            if (result.IsNotAllowed)
            {
                isPasswordCorrect = await _userManager.CheckPasswordAsync(user, dto.Password);
            }

            string code;

            if (isPasswordCorrect)
            {
                var confirmTokenDto = await GenerateCurrentEmailConfirmationToken(user);
                return Result.Ok(new AuthResultDto { UserId = user.Id, Code = confirmTokenDto.Value.Code, EmailNotConfirmed = true });
            }

            var hasPasswordResult = await _userManager.HasPasswordAsync(user);

            if (hasPasswordResult)
            {
                _logger.LogWarning("Failed to login: password incorrect for User with UserId={Id}", user.Id);
                return Result.Fail(new Error("Login failed").WithMetadata("Code", "LoginFailed"));
            }

            code = (await GeneratePasswordResetTokenAsync(dto.Email)).Value.Code;
            return Result.Ok(new AuthResultDto { UserId = user.Id, Code = code, PasswordNotExist = true });
        }

        ///  <summary>
        ///  Check email verification token
        ///  </summary>
        ///  <param name="dto">The data containing UserId and email confirmation token</param>
        ///  <returns>
        ///  <see cref="Result"/> containing:
        ///  <list type="bullet">
        ///  <item>Success if token is correct</item>
        ///  <item>Error information if user not found or token is incorrect</item>
        /// </list>
        /// </returns>
        public async Task<Result> ConfirmEmailAsync(ConfirmTokenDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                _logger.LogWarning("Failed to confirm email: User with UserId={userId} not found", dto.UserId);
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }
            dto.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Code));
            var result = await _userManager.ConfirmEmailAsync(user, dto.Code);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to confirm email: confirm token incorrect for User with UserId={UserId}", dto.UserId);
                return Result.Fail(result.Errors.Select(e => new Error(e.Description).WithMetadata("Code", e.Code)));
            }
            await _signInManager.SignInAsync(user, new AuthenticationProperties());
            return Result.Ok();
        }

        ///  <summary>
        ///  Generate password reset token
        ///  </summary>
        ///  <param name="email">User email</param>
        ///  <returns>
        ///  <see cref="ConfirmTokenDto"/> containing:
        ///  <list type="bullet">
        ///  <item>UserId and reset password token if operation successful</item>
        ///  <item>Error information if user not found or his email didn't confirmed</item>
        /// </list>
        /// </returns>
        public async Task<Result<ConfirmTokenDto>> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !user.EmailConfirmed)
            {
                _logger.LogWarning("Failed to generate password reset token: User not found");
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            return Result.Ok(new ConfirmTokenDto { Code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code)), UserId = user.Id });
        }

        ///  <summary>
        ///  Generate current email confirmation token for register confirmation
        ///  or email changing
        ///  </summary>
        ///  <param name="email">Current user email</param>
        ///  <returns>
        ///  <see cref="ConfirmTokenDto"/> containing:
        ///  <list type="bullet">
        ///  <item>UserId and reset password token if operation successful</item>
        ///  <item>Error information if user not found</item>
        /// </list>
        /// </returns>
        public async Task<Result<ConfirmTokenDto>> GenerateCurrentEmailConfirmationTokenByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Failed to generate current email confirmation token by email: User not found");
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }

            return await GenerateCurrentEmailConfirmationToken(user);
        }

        ///  <summary>
        ///  Generate current email confirmation token for email changing
        ///  </summary>
        ///  <param name="userId">The identifier of the user</param>
        ///  <returns>
        ///  <see cref="ConfirmTokenDto"/> containing:
        ///  <list type="bullet">
        ///  <item>UserId and reset password token if operation successful</item>
        ///  <item>Error information if user not found</item>
        /// </list>
        /// </returns>
        public async Task<Result<ConfirmTokenDto>> GenerateCurrentEmailConfirmationTokenByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Failed to generate current email confirmation token by id: User with UserId={userId} not found", userId);
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }

            return await GenerateCurrentEmailConfirmationToken(user);
        }

        private async Task<Result<ConfirmTokenDto>> GenerateCurrentEmailConfirmationToken(ApplicationUser user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return Result.Ok(new ConfirmTokenDto { Code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code)), UserId = user.Id });
        }

        ///  <summary>
        ///  Confirm current email or 
        ///  add pendingNewEmail for email changing if is not null
        ///  </summary>
        ///  <param name="dto">The data containing UserId and email confirmation token</param>
        ///  <returns>
        ///  <see cref="ConfirmTokenDto"/> containing:
        ///  <list type="bullet">
        ///  <item>Ok if operation successful</item>
        ///  <item>Error information if user not found or token incorrect</item>
        /// </list>
        /// </returns>
        public async Task<Result> ConfirmCurrentEmailAsync(ConfirmTokenDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                _logger.LogWarning("Failed to confirm current email: User with UserId={userId} not found", dto.UserId);
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }
            dto.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Code));
            var result = await _userManager.ConfirmEmailAsync(user, dto.Code);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to confirm current email: Confirm token incorrect for User with UserId={userId}", dto.UserId);
                return Result.Fail(new Error("Token incorrect").WithMetadata("Code", "TokenIncorrect"));
            }
            if (!string.IsNullOrWhiteSpace(dto.NewEmail) && dto.NewEmail != user.PendingNewEmail)
            {
                user.PendingNewEmail = dto.NewEmail;
                await _userManager.UpdateAsync(user);
            }

            return Result.Ok();
        }

        ///  <summary>
        ///  Generate new email confirmation token for changing email
        ///  </summary>
        ///  <param name="userId">The identifier of the user</param>
        ///  <returns>
        ///  <see cref="ConfirmTokenDto"/> containing:
        ///  <list type="bullet">
        ///  <item>UserId and new email confirmation token if operation successful</item>
        ///  <item>Error information if user not found or user don't confirm his current email</item>
        /// </list>
        /// </returns>
        public async Task<Result<ConfirmTokenDto>> GenerateNewEmailConfirmationTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Failed to generate new email confirmation token: User with UserId={userId} not found", userId);
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }

            if (!user.EmailConfirmed || user.PendingNewEmail == null)
            {
                _logger.LogWarning("Failed to generate new email confirmation token: Current email not confirmed for User with UserId={userId}", userId);
                return Result.Fail(new Error("Current email not confirmed").WithMetadata("Code", "CurrentEmailNotConfirmed"));
            }

            var code = await _userManager.GenerateChangeEmailTokenAsync(user, user.PendingNewEmail);
            return Result.Ok(new ConfirmTokenDto { Code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code)), UserId = user.Id });
        }

        ///  <summary>
        ///  Confirm new email for email changing
        ///  </summary>
        ///  <param name="dto">The data containing UserId and new email confirmation token</param>
        ///  <returns>
        ///  <see cref="Result"/> containing:
        ///  <list type="bullet">
        ///  <item>Ok if token correct and operation successful</item>
        ///  <item>Error information if user not found or user don't confirm his current email</item>
        /// </list>
        /// </returns>
        public async Task<Result> ConfirmNewEmailAsync(ConfirmTokenDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                _logger.LogWarning("Failed to confirm new email: User with UserId={userId} not found", dto.UserId);
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }

            if (user.PendingNewEmail == null)
            {
                _logger.LogWarning("Failed to confirm new email: Current email not confirmed for User with UserId={userId}", dto.UserId);
                return Result.Fail(new Error("Current email not confirmed").WithMetadata("Code", "CurrentEmailNotConfirmed"));
            }

            dto.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Code));
            var result = await _userManager.ChangeEmailAsync(user, user.PendingNewEmail, dto.Code);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to confirm new email: incorrect confirm token for User with UserId={userId} not found", dto.UserId);
                return Result.Fail(result.Errors.Select(e => new Error(e.Description).WithMetadata("Code", e.Code)));
            }

            user.UserName = user.PendingNewEmail;
            user.NormalizedUserName = user.PendingNewEmail.ToUpper();
            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);

            return Result.Ok();
        }

        ///  <summary>
        ///  Check if password existing
        ///  </summary>
        ///  <param name="userId">The identifier of the user</param>
        ///  <returns>
        ///  <see cref="bool"/> containing:
        ///  <list type="bullet">
        ///  <item>IsPasswordExist if operation successful</item>
        ///  <item>Error information if user not found</item>
        /// </list>
        /// </returns>
        public async Task<Result<bool>> HasPasswordAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Failed to has password: User with UserId={userId} not found", userId);
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }

            bool result = await _userManager.HasPasswordAsync(user);

            return Result.Ok(result);
        }

        ///  <summary>
        ///  Add exteranl login if not exist
        ///  and sign in into account
        ///  </summary>
        ///  <param name="ExternalLoginInfo"> with login provider, provider key, email and name</param>
        ///  <returns>
        ///  <see cref="ExternalLoginResultDto"/> containing:
        ///  <list type="bullet">
        ///  <item>IsLogin = true if user registered</item>
        ///  <item>IsLogin = false if user sign in or add login</item>
        ///  <item>Error information if email = null or identityResult isn't successful</item>
        /// </list>
        /// </returns>
        public async Task<Result<ExternalLoginResultDto>> ProcessExternalLoginAsync(ExternalLoginInfo info)
        {
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
            if (result.Succeeded)
            {
                return Result.Ok(new ExternalLoginResultDto { IsLogin = true });
            }
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                _logger.LogWarning("Failed to process external login: email is null");
                return Result.Fail(new Error("Invalid request").WithMetadata("Code", "InvalidRequest"));
            }
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, true);
                await _userManager.ConfirmEmailAsync(user, await _userManager.GenerateEmailConfirmationTokenAsync(user));
                return Result.Ok(new ExternalLoginResultDto { IsLogin = true });
            }
            var fullName = info.Principal.FindFirstValue(ClaimTypes.Name);
            user = new ApplicationUser { UserName = email, Email = email, FullName = fullName!, EmailConfirmed = true };
            var identityResult = await _userManager.CreateAsync(user);
            if (!identityResult.Succeeded)
            {
                _logger.LogWarning("Failed to process external login: invalid request");
                return Result.Fail(new Error("Invalid request").WithMetadata("Code", "InvalidRequest"));
            }
            identityResult = await _userManager.AddLoginAsync(user, info);

            if (!identityResult.Succeeded)
            {
                _logger.LogWarning("Failed to process external login: invalid request");
                return Result.Fail(new Error("Invalid request").WithMetadata("Code", "InvalidRequest"));
            }
            await _signInManager.SignInAsync(user, true);
            return Result.Ok(new ExternalLoginResultDto { IsLogin = false });
        }

        ///  <summary>
        ///  Edit user fullName and PhoneNumber
        ///  </summary>
        ///  <param name="UserProfileEditDto">The data containing userId, fullName and PhoneNumber</param>
        ///  <returns>
        ///  <see cref="Result"/> containing:
        ///  <list type="bullet">
        ///  <item>Ok if operation successful</item>
        ///  <item>Error information if user not found</item>
        /// </list>
        /// </returns>
        public async Task<Result> EditUserProfileAsync(UserProfileEditDto dto)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                _logger.LogWarning("Failed to edit user profile: User with UserId={UserId} not found", dto.UserId);
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }

            user.PhoneNumber = dto.PhoneNumber?.Trim();
            user.FullName = dto.FullName.Trim();
            await _userManager.UpdateAsync(user);
            return Result.Ok();
        }

        ///  <summary>
        ///  Add password if it doesn't exist
        ///  </summary>
        ///  <param name="SetPasswordDto">The data containing UserId, NewPassword and ConfirmPassword</param>
        ///  <returns>
        ///  <see cref="Result"/> containing:
        ///  <list type="bullet">
        ///  <item>Ok if operation successful</item>
        ///  <item>Error information if user not found, passwords do not match or password already exist</item>
        /// </list>
        /// </returns>
        public async Task<Result> SetPasswordAsync(SetPasswordDto dto)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                _logger.LogWarning("Failed to set password: User with UserId={UserId} not found", dto.UserId);
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }

            if (dto.NewPassword != dto.ConfirmPassword)
            {
                _logger.LogWarning("Failed to set password: Passwords do not match for User with UserId={UserId}", dto.UserId);
                return Result.Fail(new Error("Password is differents").WithMetadata("Code", "PasswordIsDifferents"));
            }

            if (await _userManager.HasPasswordAsync(user))
            {
                _logger.LogWarning("Failed to set password: password already exist for User with UserId={UserId}", dto.UserId);
                return Result.Fail(new Error("Password already exist").WithMetadata("Code", "PasswordAlreadyExist"));
            }
            var result = await _userManager.AddPasswordAsync(user, dto.ConfirmPassword);

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);

            return Result.Ok();
        }

        ///  <summary>
        ///  Change user password
        ///  </summary>
        ///  <param name="ChangePasswordDto">The data containing UserId, CurrentPassword, NewPassword and ConfirmPassword</param>
        ///  <returns>
        ///  <see cref="Result"/> containing:
        ///  <list type="bullet">
        ///  <item>Ok if operation successful</item>
        ///  <item>Error information if user not found, passwords do not match or current password incorrect</item>
        /// </list>
        /// </returns>
        public async Task<Result> ChangePasswordAsync(ChangePasswordDto dto)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                _logger.LogWarning("Failed to change password: User with UserId={UserId} not found", dto.UserId);
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }

            if (dto.NewPassword != dto.ConfirmPassword)
            {
                _logger.LogWarning("Failed to change password: Passwords do not match for User with UserId={UserId}", dto.UserId);
                return Result.Fail(new Error("Password is differents").WithMetadata("Code", "PasswordIsDifferents"));
            }

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.ConfirmPassword);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to change password: verification token incorrect for User with UserId={UserId}", dto.UserId);
                return Result.Fail(new Error("Current password incorrect").WithMetadata("Code", "CurrentPasswordIncorrect"));
            }

            await _signInManager.RefreshSignInAsync(user);

            return Result.Ok();
        }

        ///  <summary>
        ///  Change user password
        ///  </summary>
        ///  <param name="ResetPasswordDto">The data containing UserId, reset password token, NewPassword and ConfirmPassword</param>
        ///  <returns>
        ///  <see cref="Result"/> containing:
        ///  <list type="bullet">
        ///  <item>Ok if operation successful</item>
        ///  <item>Error information if user not found, passwords do not match or reset password token incorrect</item>
        /// </list>
        /// </returns>
        public async Task<Result> ResetPasswordAsync(ResetPasswordDto dto)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                _logger.LogWarning("Failed to reset password: User with UserId={UserId} not found", dto.UserId);
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }

            if (dto.Password != dto.ConfirmPassword)
            {
                _logger.LogWarning("Failed to reset password: Passwords do not match for User with UserId={UserId}", dto.UserId);
                return Result.Fail(new Error("Password is differents").WithMetadata("Code", "PasswordIsDifferents"));
            }

            var result = await _userManager.ResetPasswordAsync(user, Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Code)), dto.ConfirmPassword);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to reset password: verification token incorrect for User with UserId={UserId}", dto.UserId);
                return Result.Fail(result.Errors.Select(e => new Error(e.Description).WithMetadata("Code", "PasswordIsDifferents")));
            }

            return Result.Ok();
        }

        ///  <summary>
        ///  Get user profile
        ///  </summary>
        ///  <param name="userId">Identifier of the user</param>
        ///  <returns>
        ///  <see cref="UserProfileEditDto"/> containing:
        ///  <list type="bullet">
        ///  <item>Email, FullName, PhoneNumber, IsPasswordExist if operation successful</item>
        ///  <item>Error information if user not found</item>
        /// </list>
        /// </returns>
        public async Task<Result<UserProfileEditDto>> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.EmailConfirmed)
            {
                _logger.LogWarning("Failed to get user profile: User with UserId={id} not found", userId);
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }

            return Result.Ok(new UserProfileEditDto
            {
                Email = user.Email!,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                UserId = user.Id,
                IsPasswordExist = await _userManager.HasPasswordAsync(user)
            });
        }

        ///  <summary>
        ///  Get user fullName
        ///  </summary>
        ///  <param name="userId">Identifier of the user</param>
        ///  <returns>
        ///  <see cref="GetFullNameResultDto"/> containing:
        ///  <list type="bullet">
        ///  <item>FullName if operation successful</item>
        ///  <item>Error information if user not found</item>
        /// </list>
        /// </returns>
        public async Task<Result<GetFullNameResultDto>> GetFullNameAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Failed to get user FullName with UserId={id}", userId);
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }

            return Result.Ok(new GetFullNameResultDto { FullName = user.FullName });
        }

        ///  <summary>
        ///  Edit user fullName
        ///  </summary>
        ///  <param name="userId">Identifier of the user</param>
        ///  <param name="fullName">FullName of the user</param>
        ///  <returns>
        ///  <see cref="Result"/> containing:
        ///  <list type="bullet">
        ///  <item>Ok if operation successful</item>
        ///  <item>Error information if user not found</item>
        /// </list>
        /// </returns>
        public async Task<Result> EditFullNameAsync(string userId, string fullName)
        {
            var foundUser = await _userManager.FindByIdAsync(userId);
            if (foundUser == null)
            {
                _logger.LogWarning("Failed to edit user FullName: User with UserId={id} not found", userId);
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }

            foundUser.FullName = fullName;
            await _userManager.UpdateAsync(foundUser);

            return Result.Ok();
        }

        ///  <summary>
        ///  Find user using email
        ///  </summary>
        ///  <param name="email">Email of the user</param>
        ///  <returns>
        ///  <see cref="FindUserByEmailDto"/> containing:
        ///  <list type="bullet">
        ///  <item>Id if operation successful</item>
        ///  <item>Error information if user not found</item>
        /// </list>
        /// </returns>
        public async Task<Result<FindUserByEmailDto>> FindUserByEmailAsync(string email)
        {
            string emailNormalized = email.ToLower();
            ApplicationUser? foundUser = await _userManager.FindByEmailAsync(email);

            if (foundUser == null)
            {
                _logger.LogWarning("Failed to find user my email: User not found");
                return Result.Fail(new Error("User not found").WithMetadata("Code", "UserNotFound"));
            }

            if (!foundUser.EmailConfirmed)
            {
                _logger.LogWarning("Failed to find user my emai: Current email not confirmed for User with UserId={userId}", foundUser.Id);
                return Result.Fail(new Error("Current email not confirmed").WithMetadata("Code", "CurrentEmailNotConfirmed"));
            }

            return Result.Ok(new FindUserByEmailDto { UserId = foundUser.Id });
        }

        ///  <summary>
        ///  Get users for administrator
        ///  </summary>
        ///  <param name="page">Page number starting from 1</param>
        ///  <param name="pageSize">Number of users to receive</param>
        ///  <returns>
        ///  <see cref="UsersDto"/> containing:
        ///  <list type="bullet">
        ///  <item>List of users that containing FullName, Email, Status and Role if operation successful</item>
        ///  <item>Error information if it fails</item>
        /// </list>
        /// </returns>
        public async Task<Result<UsersDto>> GetUsers(int page, int pageSize)
        {
            return _mapper.Map<IEnumerable<Domain.Models.User>, UsersDto>(await _userRepository.GetUsers(page, pageSize));
        }
    }
}