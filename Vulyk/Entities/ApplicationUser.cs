using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Vulyk.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(20)]
        public string? FullName { get; set; }

        public DateTime? LastOnline { get; set; } = DateTime.Now;

        public string? PendingNewEmail { get; set; }

        public IEnumerable<UserChat> UserChats { get; set; } = new List<UserChat>();
    }
}
