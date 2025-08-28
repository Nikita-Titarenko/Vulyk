namespace Vulyk.Web.ApiModels.Requests
{
    public class CreateMessageRequestModel
    {
        public string PartnerId { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;
    }
}
