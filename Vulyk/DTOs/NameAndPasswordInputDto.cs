using System.ComponentModel.DataAnnotations;

namespace Vulyk.DTOs
{
    public class NameAndPasswordInputDto
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
