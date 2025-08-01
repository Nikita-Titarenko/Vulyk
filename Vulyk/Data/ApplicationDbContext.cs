using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vulyk.Models;

namespace Vulyk.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<User> User { get; set; }

        public DbSet<Chat> Chat { get; set; }

        public DbSet<UserChat> UserChat { get; set; }

        public DbSet<Message> Message { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().Property(u => u.FullName).HasMaxLength(20);

            builder.Entity<User>().Property(u => u.Password).HasMaxLength(20);

            builder.Entity<User>().Property(u => u.Phone).HasMaxLength(20);

            builder.Entity<User>().Property(u => u.Email).HasMaxLength(320);

            builder.Entity<User>().Property(u => u.VerificationCode).HasMaxLength(6);

            builder.Entity<Message>().Property(m => m.Text).HasMaxLength(1000);

            builder.Entity<UserChat>().HasKey(uc => new { uc.UserId, uc.ChatId });

            builder.Entity<Message>().HasOne(m => m.UserChat).WithMany(m => m.Messages).HasForeignKey(uc => new { uc.UserId, uc.ChatId }).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
