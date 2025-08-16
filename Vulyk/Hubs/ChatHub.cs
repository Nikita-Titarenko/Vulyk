using Microsoft.AspNetCore.SignalR;
using Vulyk.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Vulyk.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }
        public async Task LoadChatsAsync(List<string> chatIds)
        {
            try
            {
                foreach (var chatId in chatIds)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
                }
            }
            catch
            {
                _logger.LogError("Failed to load chats");
                throw;
            }
        }

        public async Task LoadChatAsync(string chatId)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            }
            catch
            {
                _logger.LogError("Failed to load chats with chatId={chatId}", chatId);
                throw;
            }
        }

        public async Task SendMessageAsync(string chatId, string userId, string text)
        {
            try
            {
                await Clients.Groups(chatId).SendAsync("ReceiveMessage", userId, text);
            }
            catch
            {
                _logger.LogError("Failed to send message with chatId={chatId} and userId={userId}", chatId, userId);
                throw;
            }

        }

        public async Task JoinUserGroupAsync(string userId)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
            }
            catch
            {
                _logger.LogError("Failed to add user to group with userId={userId}", userId);
                throw;
            }

        }

        public async Task CreateChatAsync(string userId, string partnerId, int chatId, string fullName, string lastMessage)
        {
            try
            {
                await Clients.Groups($"user-{partnerId}").SendAsync("CreateChat", userId, chatId, fullName, lastMessage);
            }
            catch
            {
                _logger.LogError("Failed to create chat with userId={userId}, partnerId={partnerId} and chatId={chatId}", userId, partnerId, chatId);
                throw;
            }

            await LoadChatAsync(chatId.ToString());
        }
    }
}