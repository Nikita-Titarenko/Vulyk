using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Vulyk.Domain.Models;

namespace Vulyk.Infrastructure.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(20)]
        public string FullName { get; set; } = string.Empty;

        public DateTime? LastOnline { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? PendingNewEmail { get; set; }

        public IEnumerable<UserChat> UserChats { get; set; } = new List<UserChat>();
    }
}