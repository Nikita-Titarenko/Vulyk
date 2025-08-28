using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Vulyk.Application.Repositories;
using Vulyk.Application.Services.Chat;
using Vulyk.Application.Services.JwtToken;
using Vulyk.Application.Services.Message;
using Vulyk.Application.Services.User;
using Vulyk.Infrastructure.Repositories;
using Vulyk.Infrastructure.Services.Email;
using Vulyk.Infrastructure.Services.JwtToken;
using Vulyk.Infrastructure.Services.User;

namespace Vulyk.Infrastructure
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddHostedService<DeleteUnconfirmedUsersService>();
            services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
            return services;
        } 
    }
}