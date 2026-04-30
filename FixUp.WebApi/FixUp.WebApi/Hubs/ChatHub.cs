using FixUp.Service.DTOs;
using FixUp.Service.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FixUp.WebAPI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task SendMessage(MessageDTO message)
        {
            // שמירה ב-DB
            await _messageService.AddAsync(message);

            // שליחה רק למשתתפים שנמצאים בקבוצה של השיחה הזו
            await Clients.Group(message.ConversationId).SendAsync("ReceiveMessage", message);
        }

        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        public async Task JoinCategoryGroup(int categoryId)
        {
            string groupName = $"Category_{categoryId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Category_{categoryId}");
            Console.WriteLine($"--- SignalR: Connection {Context.ConnectionId} joined group {groupName} ---");
        }

        public override async Task OnConnectedAsync()
        {
            var categoryIdClaim = Context.User?.FindFirst("CategoryId")?.Value;
            var roleClaim = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

            // 2. רק אם הוא בעל מקצוע ויש לו קטגוריה, נצרף אותו לקבוצה
            if (roleClaim == "Professional" && !string.IsNullOrEmpty(categoryIdClaim))
            {
                string groupName = $"Category_{categoryIdClaim}";
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                Console.WriteLine($"Connection {Context.ConnectionId} joined group: {groupName}");
            }

            await base.OnConnectedAsync();
        }
    }
}