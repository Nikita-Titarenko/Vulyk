using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vulyk.DTOs
{
    public class UserEditDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public string? Phone { get; set; }
        [Required]
        public string FullName { get; set; } = string.Empty;
    }
}
