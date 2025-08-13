using Microsoft.AspNetCore.SignalR;
using Vulyk.Entities;
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

        public async Task SendMessageAsync(string chatId, string userId, string text)
        {
            await Clients.Groups(chatId).SendAsync("ReceiveMessage", userId, text);
        }

        public async Task JoinUserGroupAsync(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }

        public async Task CreateChatAsync(string userId, string partnerId, int chatId, string fullName, string lastMessage)
        {
            await Clients.Groups($"user-{partnerId}").SendAsync("CreateChat", userId, chatId, fullName, lastMessage);
            await LoadChatAsync(chatId.ToString());
        }
    }
}