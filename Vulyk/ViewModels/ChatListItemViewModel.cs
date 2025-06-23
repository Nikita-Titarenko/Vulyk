namespace Vulyk.ViewModels
{
    public class ChatListItemViewModel
    {
        public int ChatId { get; set; }

        public int UserId { get; set; }

        public string Name = string.Empty;

        public DateTime? LastMessageDateTime { get; set; }

        public string? LastMessageText { get; set; }

        public string? GetLastMessageDateForChat()
        {
            if (LastMessageDateTime == null)
            {
                return null;
            }
            DateTime last = LastMessageDateTime.Value;
            if (last.Date != DateTime.Now.Date)
            {
                if (last.Date.AddDays(7) < DateTime.Now.Date)
                {
                    return last.ToString("dd.MM.yy");
                } else
                {
                    return last.ToString("ddd");
                }
            } else
            {
                return last.ToString("HH:mm");
            }
        }
    }
}
