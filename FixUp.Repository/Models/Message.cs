using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



    
namespace YourProjectName.Models {
        public class Message
        {
            public int Id { get; set; }
            public string Content { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public int SenderId { get; set; }
            public string SenderName { get; set; }
            public string SenderRole { get; set; } // "pro" או "client"
            public int CategoryId { get; set; }
            public string? ImageUrl { get; set; } // סימן שאלה כי לא חובה להעלות תמונה בכל הודעה
        }
}

