using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Vulyk.Web.ViewModels.UserManagement
{
    public class UserViewModel
    {
        public string Email { get; set; } = string.Empty;
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserStatus Status { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
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
