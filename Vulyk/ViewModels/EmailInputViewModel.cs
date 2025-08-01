using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;

namespace Vulyk.ViewModels
{
    public class EmailInputViewModel : BaseEmailViewModel
    {
        [Required]
        [EmailAddress]
        public new string Email { get => base.Email; set => base.Email = value; }
    }
}