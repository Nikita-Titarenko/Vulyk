using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vulyk.ViewModels
{
    public class EmailAndPasswordInputViewModel : BaseEmailViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "The password length needs to be from 10 to 20 characters")]
        public string Password { get; set; } = string.Empty;
    }
}