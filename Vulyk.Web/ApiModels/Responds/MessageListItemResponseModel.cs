namespace Vulyk.Web.ApiModels.Responds
{
    public class MessageListItemResponseModel
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreationDateTime { get; set; }
        public bool IsMine { get; set; }
    }
}
