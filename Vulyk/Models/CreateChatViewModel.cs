using System.ComponentModel.DataAnnotations;

namespace Vulyk.Models
{
    public class CreateChatViewModel
    {
        [Display(Name = "Your login")]
        public string Login { get; set; } = string.Empty;
        [Display(Name = "Login")]
        public string LoginToAdd { get; set; } = string.Empty;
        [Display(Name = "Phone")]
        public string phoneToAdd { get; set; } = string.Empty;
        public bool IsUseLogin { get; set; }
    }
}
