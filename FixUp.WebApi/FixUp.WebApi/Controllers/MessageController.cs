using FixUp.Service.DTOs;
using FixUp.Service.Interfaces;
using FixUp.WebAPI.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FixUp.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _hubContext; // הוספת ה-Hub

        public MessagesController(IMessageService messageService, IHubContext<ChatHub> hubContext)
        {
            _messageService = messageService;
            _hubContext = hubContext;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { url = $"https://localhost:7230/images/{fileName}" });
        }

        [HttpPost("send")]
        
        public async Task<IActionResult> SendMessage([FromBody] MessageDTO messageDto)
        {
            if (messageDto == null || string.IsNullOrEmpty(messageDto.Content))
            {
                return BadRequest("תוכן ההודעה לא יכול להיות ריק");
            }

            // 1. שמירה במסד הנתונים
            await _messageService.AddAsync(messageDto);

            // 2. הפצת ההודעה המקורית לכל מי שמחובר (בזמן אמת)
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", messageDto);

            // 3. יצירת תגובה אוטומטית (בוט)
            var autoReply = new MessageDTO
            {
                Content = "קיבלנו את הודעתך, נחזור אליך בהקדם!",
                SenderName = "מערכת FixUp",
                CreatedAt = DateTime.Now,
                // ודאי שה-SenderId כאן שונה מה-ID של המשתמש כדי שהבועה תהיה בצד השני
                SenderId = 0
            };

            // שליחת התגובה האוטומטית לכולם
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", autoReply);

            return Ok(new { status = "Success" });
        }
    }
}