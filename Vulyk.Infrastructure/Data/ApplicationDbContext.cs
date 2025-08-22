using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vulyk.Domain.Models;
using Vulyk.Infrastructure.Models;

namespace Vulyk.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.Entity<ApplicationUser>().Property(u => u.FullName).HasMaxLength(20);

            builder.Entity<Message>().Property(m => m.Text).HasMaxLength(1000);

            builder.Entity<UserChat>().HasKey(uc => new { uc.UserId, uc.ChatId });

            builder.Entity<UserChat>().HasOne<ApplicationUser>().WithMany().HasForeignKey(uc => uc.UserId).HasPrincipalKey(uc => uc.Id).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserChat>().HasOne(m => m.Chat).WithMany(m => m.UserChats).HasForeignKey(uc => uc.ChatId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>().HasOne(m => m.UserChat).WithMany(m => m.Messages).HasForeignKey(uc => new { uc.UserId, uc.ChatId }).OnDelete(DeleteBehavior.Cascade);
        }
    }
}