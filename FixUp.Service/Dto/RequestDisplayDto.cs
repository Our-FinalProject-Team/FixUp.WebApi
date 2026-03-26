using System;

namespace FixUp.Service.Dto
{
    public class RequestDisplayDto
    {
        public int Id { get; set; }
        public string Subject { get; set; }      // הקטגוריה (אינסטלציה וכו')
        public string Description { get; set; }  // תיאור התקלה
        public string Address { get; set; }      // איפה העבודה
        public string Status { get; set; }       // "חדש", "בטיפול", "הסתיים"
        public DateTime CreatedAt { get; set; }  // מתי נוצרה הבקשה
        public DateTime ScheduledDate { get; set; } // מתי בעל המקצוע אמור להגיע

        // שדות תצוגה חשובים (נשלפים מהטבלאות האחרות ב-Service)
        public string ClientName { get; set; }
        public string ProfessionalName { get; set; }
    }
}