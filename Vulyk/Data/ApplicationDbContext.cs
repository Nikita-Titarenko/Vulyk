﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
            builder.Entity<UserChat>().HasKey(uc => new { uc.UserId, uc.ChatId });

            builder.Entity<Message>().HasOne(m => m.UserChat).WithMany(m => m.Messages).HasForeignKey(uc => new { uc.UserId, uc.ChatId }).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
