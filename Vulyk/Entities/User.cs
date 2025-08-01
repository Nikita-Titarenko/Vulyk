using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Vulyk.Models
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Phone), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
        [MaxLength(320)]
        public string Email { get; set; } = string.Empty;
        public string? ProviderUserId { get; set; }
        public RegisterStatus RegisterStatus { get; set; }
        [MaxLength(6)]
        public string? VerificationCode { get; set; }
        public DateTime? ExpirationTime { get; set; }
        [MaxLength(20)]
        public string? Password { get; set; }
        [MaxLength(20)]
        public string? Phone { get; set; }
        [MaxLength(20)]
        public string? FullName { get; set; }

        public DateTime? LastOnline { get; set; } = DateTime.Now;

        public IEnumerable<UserChat> UserChats { get; set; } = new List<UserChat>();
    }

    public enum RegisterStatus
    {
        EmailInputted, VerificationCodeConfirmed, Registered
    }
}
