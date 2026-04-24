using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



    
namespace YourProjectName.Models
    {
        public class Message
        {
            public int Id { get; set; }
            public string? Content { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public string ConversationId { get; set; } 
            public int SenderId { get; set; }
            public string SenderName { get; set; }
            public string SenderRole { get; set; } 
            public int CategoryId { get; set; }
            public string? ImageUrl { get; set; } 
        }
    }

