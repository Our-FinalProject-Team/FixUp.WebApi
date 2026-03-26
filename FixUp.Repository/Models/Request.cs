

public class Request
{
    public int Id { get; set; }
    public int ClientId { get; set; } // הלקוח שהזמין
    public int? ProfessionalId { get; set; } // בעל המקצוע שנבחר
    public string Address { get; set; } // כתובת ספציפית להזמנה
    public DateTime ScheduledDate { get; set; } // התאריך שהלקוח בחר
    public string Subject { get; set; } // למשל "אינסטלציה"
    public string Description { get; set; } // הערות
    public string Status { get; set; } = "חדש"; // סטטוס ראשוני
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

//namespace FixUp.Repository.Models
//{
  
//    public class Request
//    {
//        public int Id { get; set; }
//        public int ClientId { get; set; }
//        public int? ProfessionalId { get; set; } // יכול להיות null בהתחלה
//        public string Subject { get; set; }
//        public string Description { get; set; }
//        public string ImageUrl { get; set; }
//        public DateTime CreatedAt { get; set; }
//        public string Status { get; set; }     
//    }
//}

