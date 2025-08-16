namespace Vulyk.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(20)]
        public string FullName { get; set; } = string.Empty;

        public DateTime? LastOnline { get; set; } = DateTime.Now;

        public string? PendingNewEmail { get; set; }

        public IEnumerable<UserChat> UserChats { get; set; } = new List<UserChat>();
    }
}