namespace Vulyk.ApiModels.Responds
{
    public class ProfileResponseModel
    {
        public string FullName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public bool IsPasswordExist { get; set; }

        public string Email { get; set; } = string.Empty;
    }
}
