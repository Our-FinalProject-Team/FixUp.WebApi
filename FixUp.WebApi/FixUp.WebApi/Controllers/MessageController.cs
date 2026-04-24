using Azure.Core;
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

        // שליפת היסטוריית הודעות לפי קטגוריה (פורום ספציפי)
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetByCategoryId(int categoryId)
        {
            var messages = await _messageService.GetByCategoryIdAsync(categoryId);
            return Ok(messages);
        }

        //[HttpPost("send")]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> SendMessage([FromForm] IFormFile? image,[FromForm] MessageDTO messageDto)
        //{
        //    if (messageDto == null || string.IsNullOrEmpty(messageDto.Content))
        //    {
        //        return BadRequest("תוכן ההודעה לא יכול להיות ריק");
        //    }

        //    Console.WriteLine("DTO ConversationId: " + messageDto.ConversationId);

        //    //ניתוח התמונה
        //    var categoryResult = await _analysisService.AnalyzeRequestAsync(image, messageDto.Content);
        //    if (categoryResult != null)
        //    {
        //        messageDto.CategoryId = categoryResult.CategoryId;
        //        messageDto.SenderRole = categoryResult.CategoryName;
        //    }

        //    var convId = Request.Form["ConversationId"].ToString();
        //    if(convId == null)
        //        Console.WriteLine("null");
        //    messageDto.ConversationId = convId;

        //    // 1. שמירה ב-DB (הפעולה מחזירה void/Task אז לא שומרים במשתנה)
        //    await _messageService.AddAsync(messageDto);


        //    await _hubContext.Clients.All.SendAsync("ReceiveMessage", messageDto);

        //    return Ok(new { message = messageDto, analysis = categoryResult });
        //}
        [HttpPost("send")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SendMessage()
        {
            var form = Request.Form;

            // 🔍 בדיקות בסיסיות
            var content = form["Content"].ToString();
            if (string.IsNullOrWhiteSpace(content) && form.Files.Count == 0)
            {
                return BadRequest("תוכן ההודעה לא יכול להיות ריק");
            }

            // 🔥 בניית DTO ידנית (עוקף את כל בעיות הביינדינג)
            var messageDto = new MessageDTO
            {
                Content = content,
                ConversationId = form["ConversationId"],
                SenderName = form["SenderName"],
                SenderRole = form["SenderRole"],
                ImageUrl = null,
                CreatedAt = DateTime.Now
            };

            // המרות מספרים (עם הגנה מקריסות)
            if (int.TryParse(form["SenderId"], out int senderId))
                messageDto.SenderId = senderId;

            if (int.TryParse(form["CategoryId"], out int categoryId))
                messageDto.CategoryId = categoryId;

            // 📎 קובץ (אם קיים)
            var image = form.Files.FirstOrDefault();

            

            // 🧠 ניתוח (אם יש לך שירות כזה)
            var categoryResult = await _analysisService.AnalyzeRequestAsync(image, messageDto.Content);
            if (categoryResult != null)
            {
                messageDto.CategoryId = categoryResult.CategoryId;
            }

            // 💾 שמירה
            await _messageService.AddAsync(messageDto);

            // 📡 שידור
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", messageDto);

            return Ok(new { message = messageDto, analysis = categoryResult });
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