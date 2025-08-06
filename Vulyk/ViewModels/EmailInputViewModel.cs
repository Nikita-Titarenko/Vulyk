using System.ComponentModel.DataAnnotations;

namespace Vulyk.ViewModels
{
    public class EmailInputViewModel : EmailViewModel
    {
        [Required]
        [EmailAddress]
        public new string Email { get => base.Email; set => base.Email = value; }
    }
}