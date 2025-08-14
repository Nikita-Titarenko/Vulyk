using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Vulyk.Data
{
    public class DBInitializer
    {
        public static async Task Initialize(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            db.Database.Migrate();
            for (int i = 0; i < 100; i++)
            {
                string email = $"Test{i}@gmail.com";
                if (await userManager.FindByEmailAsync(email) != null)
                {
                    continue;
                }
                ApplicationUser user = new ApplicationUser
                {
                    FullName = $"Test{i}",
                    Email = email,
                    UserName = $"Test{i}@gmail.com",
                    PhoneNumber = "+380953589545"
                };
                await userManager.CreateAsync(user, "77228Glnik!");
                await userManager.AddToRoleAsync(user, "User");
            }
        }
    }
}
