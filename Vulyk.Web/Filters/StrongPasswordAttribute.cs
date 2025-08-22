using System.ComponentModel.DataAnnotations;

namespace Vulyk.Web.Filters
{
    public class StrongPasswordAttribute : RegularExpressionAttribute
    {
        public StrongPasswordAttribute() : base(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*]).*$")
        {
            ErrorMessage = "The password needs to have digit, upper and lower case letters and unique symbols";
        }
    }
}
