namespace Vulyk.Domain.Models
{
    public class User
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
        public UserRole Role { get; set; }
        public IEnumerable<UserChat> UserChats { get; set; } = new List<UserChat>();
    }

    public enum UserStatus
    {
        ConfirmedEmail, NotConfirmedEmail
    }

    public enum UserRole
    {
        User, Admin
    }
}
