using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vulyk.DTOs
{
    public class UserProfileEditDto
    {
        public string UserId { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string FullName { get; set; } = string.Empty;
        public bool IsPasswordExist { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
