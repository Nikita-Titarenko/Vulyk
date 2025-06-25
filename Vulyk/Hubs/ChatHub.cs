using Microsoft.AspNetCore.SignalR;

namespace Vulyk.Hubs
{
    public class ChatHub : Hub
    {
        public async Task LoadChats(List<string> chatIds)
        {
            foreach (var chatId in chatIds)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            }
        }
    }
}
