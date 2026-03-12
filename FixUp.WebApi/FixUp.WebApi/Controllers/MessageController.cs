using Microsoft.AspNetCore.Mvc;
using FixUp.Service.Interfaces;
using FixUp.Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FixUp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        // שליפת כל ההודעות (לפי הממשק הכללי IService)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetAll()
        {
            var messages = await _messageService.GetAllAsync();
            return Ok(messages);
        }

        // שליפת היסטוריית הודעות לפי קטגוריה (פורום ספציפי)
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetByCategoryId(int categoryId)
        {
            var messages = await _messageService.GetByCategoryIdAsync(categoryId);
            return Ok(messages);
        }

        // שליחת הודעה חדשה
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] MessageDTO messageDto)
        {
            if (messageDto == null || string.IsNullOrEmpty(messageDto.Content))
            {
                return BadRequest("תוכן ההודעה לא יכול להיות ריק");
            }

            await _messageService.AddAsync(messageDto);
            return Ok(new { message = "ההודעה נשלחה בהצלחה" });
        }
    }
}