using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Vulyk.Web
{
    public static class ApplicationWebRegistration
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            services.AddRazorPages(opt =>
            {
                opt.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
            });
            return services;
        }
    }
}