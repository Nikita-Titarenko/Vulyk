using System.ComponentModel.DataAnnotations.Schema;

namespace Vulyk.DTOs
{
    public class UserEditDto
    {
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
