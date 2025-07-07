using System.ComponentModel.DataAnnotations;

namespace Vulyk.DTOs
{
    public class NameAndPasswordInputDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
