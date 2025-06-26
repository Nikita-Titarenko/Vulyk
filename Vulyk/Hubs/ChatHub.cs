using Microsoft.AspNetCore.SignalR;
using Vulyk.Models;
using static System.Net.Mime.MediaTypeNames;

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

        public async Task CreateChat(string chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        }

        public async Task SendMessage(string chatId, int userId, string text)
        {
            await Clients.Groups(chatId).SendAsync("ReceiveMessage", userId, text);
        }
    }
}
