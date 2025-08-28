using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vulyk.Domain.Models;
using Vulyk.Infrastructure.Models;

namespace Vulyk.Infrastructure.Services.User
{
    public class DeleteUnconfirmedUsersService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ApplicationUser> _logger;

        public DeleteUnconfirmedUsersService(IServiceProvider serviceProvider, ILogger<ApplicationUser> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                    var expirationDate = DateTime.UtcNow.AddDays(-7);
                    try
                    {
                        await userManager.Users
                            .Where(u => !u.EmailConfirmed && u.CreatedAt <= expirationDate)
                            .ExecuteDeleteAsync();
                    }
                    catch
                    {
                        _logger.LogError("Failed to delete users");
                    }
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
