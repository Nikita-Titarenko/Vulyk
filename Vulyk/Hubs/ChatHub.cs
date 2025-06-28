using Microsoft.AspNetCore.SignalR;
using Vulyk.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Vulyk.Hubs
{
    public class ChatHub : Hub
    {
        public async Task LoadChatsAsync(List<string> chatIds)
        {
            foreach (var chatId in chatIds)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            }
        }

        public async Task LoadChatAsync(string chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        }

        public async Task SendMessageAsync(string chatId, int userId, string text)
        {
            await Clients.Groups(chatId).SendAsync("ReceiveMessage", userId, text);
        }

        public async Task JoinUserGroupAsync(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }

        public async Task CreateChatAsync(string userId, int yourUserId, int chatId, string name, string lastMessage)
        {
            await Clients.Groups($"user-{userId}").SendAsync("CreateChat", yourUserId, chatId, name, lastMessage);
            await LoadChatAsync(chatId.ToString());
        }
    }
}