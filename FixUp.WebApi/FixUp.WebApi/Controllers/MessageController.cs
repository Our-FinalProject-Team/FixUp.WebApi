using Microsoft.AspNetCore.Mvc;
using FixUp.Service.Interfaces;
using FixUp.Service.DTOs;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
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

        [HttpGet("history")]
        public async Task<IActionResult> GetChatHistory()
        {
            // שליפת כל ההודעות מהשירות שמתחבר ל-SQL
            var messages = await _messageService.GetAllAsync();
            return Ok(messages);
        }

        // העלאת תמונה לשרת
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("לא נבחר קובץ.");
            }

            try
            {
                // יצירת נתיב לתיקיית wwwroot/uploads
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // יצירת שם ייחודי לקובץ
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // שמירת הקובץ
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // החזרת הכתובת היחסית של הקובץ
                var fileUrl = $"/uploads/{fileName}";
                return Ok(new { url = fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"שגיאה בהעלאת הקובץ: {ex.Message}");
            }
        }
    }
}