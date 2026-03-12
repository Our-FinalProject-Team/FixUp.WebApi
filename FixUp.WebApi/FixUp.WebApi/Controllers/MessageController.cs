using FixUp.Service.DTOs;
using FixUp.Service.Interfaces;
using FixUp.WebAPI.Hubs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FixUp.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _hubContext; // הוספת ה-Hub

        {
            _messageService = messageService;
            _hubContext = hubContext;
        }

        {

            {
        }

        // שליחת הודעה חדשה
        [HttpPost("send")]
        
        public async Task<IActionResult> SendMessage([FromBody] MessageDTO messageDto)
        {
            if (messageDto == null || string.IsNullOrEmpty(messageDto.Content))
            {
                return BadRequest("תוכן ההודעה לא יכול להיות ריק");
            }

            // 1. שמירה במסד הנתונים
            await _messageService.AddAsync(messageDto);
        }
    }
}