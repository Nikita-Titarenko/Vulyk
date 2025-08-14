using System.ComponentModel;

namespace Vulyk.ViewModels
{
    public class UserViewModel
    {
        public string Email { get; set; } = string.Empty;
        [DisplayName("Full Name")]
        public string FullName { get; set; } = string.Empty;

        public UserStatus Status { get; set; }

        public UserRole Role { get; set; }
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
