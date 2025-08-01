using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vulyk.DTOs
{
    public class UserEditDto
    {
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}
