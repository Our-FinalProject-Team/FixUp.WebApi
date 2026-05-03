using Azure.Core;
using FixUp.Service.Dto;
using FixUp.Service.DTOs;
using FixUp.Service.Interfaces;
using FixUp.Service.Interfases;
using FixUp.Service.Services;
using FixUp.WebAPI.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Mscc.GenerativeAI;
using Mscc.GenerativeAI.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FixUp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IAnalysisService _analysisService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IConfiguration _config;

        public MessageController(IMessageService messageService, IHubContext<ChatHub> hubContext, IConfiguration config, IAnalysisService analysisService)
        {
            _messageService = messageService;
            _hubContext = hubContext;
            _config = config;
            _analysisService = analysisService;
        }

        // שליפת כל ההודעות (לפי הממשק הכללי IService)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetAll()
        {
            var messages = await _messageService.GetAllAsync();
            return Ok(messages);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetByCategoryId(int categoryId)
        {
            var messages = await _messageService.GetByCategoryIdAsync(categoryId);
            return Ok(messages);
        }


        [HttpPost("send")]
        [Consumes("multipart/form-data")]

        public async Task<IActionResult> SendMessage()
        {
            var form = Request.Form;
            var content = form["Content"].ToString();

            if (string.IsNullOrWhiteSpace(content) && form.Files.Count == 0)
                return BadRequest("תוכן ההודעה לא יכול להיות ריק");

            var messageDto = new MessageDTO
            {
                Content = content,
                ConversationId = form["ConversationId"],
                SenderName = form["SenderName"],
                SenderRole = form["SenderRole"],
                CreatedAt = DateTime.Now,
                CategoryId = 0 // אתחול כברירת מחדל ל-0
            };

            if (int.TryParse(form["SenderId"], out int sId)) messageDto.SenderId = sId;

            // אם הגיע קטגוריה מהטופס (למשל מצד בעל המקצוע), נשמור אותה
            //if (int.TryParse(form["CategoryId"], out int cId)) messageDto.CategoryId = cId;
            var image = form.Files.FirstOrDefault();

            // 🧠 ניתוח AI - רק אם ההודעה ארוכה מספיק או שיש תמונה
            if (image != null || (!string.IsNullOrWhiteSpace(content) && content.Length > 10))
            {
                try
                {
                    var categoryResult = await _analysisService.AnalyzeRequestAsync(image, messageDto.Content);

                    // בדיקה קפדנית: רק אם ה-AI החזיר מספר חיובי שאינו "קוד אי-זיהוי"
                    if (categoryResult != null && categoryResult.CategoryId > 0)
                    {
                        messageDto.CategoryId = categoryResult.CategoryId;
                    }
                    else
                    {
                        messageDto.CategoryId = 0; // וידוא שזה נשאר 0 אם אין זיהוי
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ AI Analysis failed: {ex.Message}");
                    messageDto.CategoryId = 0;
                }
            }

            // 💾 שמירה בבסיס הנתונים
            await _messageService.AddAsync(messageDto);

            // 📡 שידור ל-SignalR
            try
            {
                // אם יש קטגוריה תקינה - שידור כעבודה חדשה
                if (messageDto.CategoryId > 0)
                {
                    string groupName = $"Category_{messageDto.CategoryId}";
                    await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNewJob", new
                    {
                        id = messageDto.Id,
                        conversationId = messageDto.ConversationId,
                        content = messageDto.Content,
                        senderName = messageDto.SenderName,
                        categoryId = messageDto.CategoryId,
                        createdAt = messageDto.CreatedAt
                    });
                }

                // תמיד נשדר לשיחה הספציפית כדי שהלקוח והצד השני יראו את ההודעה במסך הצ'אט
                await _hubContext.Clients.Group(messageDto.ConversationId).SendAsync("ReceiveMessage", messageDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔴 SignalR broadcast failed: {ex.Message}");
            }

            return Ok(new { success = true, message = messageDto });
        }

        [Authorize]
        [HttpGet("history/{conversationId}")]
        public async Task<IActionResult> GetChatHistory(string conversationId)
        {
            var history = await _messageService.GetMessagesIdAsync(conversationId);

            return Ok(history);
        }


        [HttpPost("analyze")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Analyze(IFormFile? image, [FromForm] string prompt)
        {
            if (string.IsNullOrEmpty(prompt))
                return BadRequest("Missing data");

            // שורה אחת שמפעילה את כל הקסם
            var result = await _analysisService.AnalyzeRequestAsync(image, prompt);

            return Ok(result);
        }


        // העלאת תמונה לשרת
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage( IFormFile image)
        {
            if (image == null || image.Length == 0)
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
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // שמירת הקובץ
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
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