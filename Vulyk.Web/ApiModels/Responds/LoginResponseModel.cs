namespace Vulyk.Web.ApiModels.Responds
{
    public class LoginResponseModel
    {
        public string UserId { get; set; } = string.Empty;

        public string JwtToken { get; set; } = string.Empty;
    }
}
