using Microsoft.AspNetCore.SignalR;
using FixUp.Service.DTOs;
using FixUp.Service.Interfaces;

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
            // שמירה וזיהוי קטגוריה ב-Service
            await _messageService.AddAsync(message);

            // שליחה לכל המחוברים בזמן אמת
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}