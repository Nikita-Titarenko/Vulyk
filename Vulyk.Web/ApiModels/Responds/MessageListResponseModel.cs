using Vulyk.Web.ViewModels.Message;

namespace Vulyk.Web.ApiModels.Responds
{
    public class MessageListResponseModel
    {
        public int? ChatId { get; set; }
        public string PartnerId { get; set; } = string.Empty;
        public List<MessageListItemViewModel> Messages { get; set; } = new List<MessageListItemViewModel>();
        public string FullName { get; set; } = string.Empty;
    }
}
