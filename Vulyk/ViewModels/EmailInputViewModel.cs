using System.ComponentModel.DataAnnotations;

namespace Vulyk.ViewModels
{
    public class EmailInputViewModel
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
