using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vulyk.Entities;

namespace Vulyk.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public DbSet<Chat> Chat { get; set; }

        public DbSet<UserChat> UserChat { get; set; }

        public DbSet<Message> Message { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            string userRoleId = Guid.NewGuid().ToString();
            string administratorRoleId = Guid.NewGuid().ToString();
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = userRoleId,
                    Name = "User",
                    NormalizedName = "USER"
                },
                new IdentityRole
                {
                    Id = administratorRoleId,
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR"
                }
            );
            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
            string administratorId = Guid.NewGuid().ToString();
            builder.Entity<ApplicationUser>().HasData(new ApplicationUser
            {
                EmailConfirmed = true,
                UserName = "vulyk.messenger@gmail.com",
                NormalizedUserName = "VULYK.MESSENGER@GMAIL.COM",
                Email = "vulyk.messenger@gmail.com",
                NormalizedEmail = "VULYK.MESSENGER@GMAIL.COM",
                FullName = "Mykyta Titarenko",
                Id = administratorId,
                PasswordHash = passwordHasher.HashPassword(new ApplicationUser(), "77228Glnik!"),
                PhoneNumber = "+380953589545"
            });

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                UserId = administratorId,
                RoleId = administratorRoleId
            });

            builder.Entity<ApplicationUser>().Property(u => u.FullName).HasMaxLength(20);

            builder.Entity<Message>().Property(m => m.Text).HasMaxLength(1000);

            builder.Entity<UserChat>().HasKey(uc => new { uc.UserId, uc.ChatId });

            builder.Entity<UserChat>().HasOne(m => m.ApplicationUser).WithMany(m => m.UserChats).HasForeignKey(uc => uc.UserId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserChat>().HasOne(m => m.Chat).WithMany(m => m.UserChats).HasForeignKey(uc => uc.ChatId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>().HasOne(m => m.UserChat).WithMany(m => m.Messages).HasForeignKey(uc => new { uc.UserId, uc.ChatId }).OnDelete(DeleteBehavior.Cascade);
        }
    }
}