//using System;

//namespace FixUp.Service.Dto
//{
//    public class RequestCreateDto
//    {
//        public int ClientId { get; set; }        // מזהה הלקוח המחובר
//        public int ProfessionalId { get; set; }  // המזהה של איש המקצוע שנבחר
//        public int CategoryId { get; set; }      // הוספנו את זה! המזהה של הקטגוריה (1, 2, 3...)
//        public string Address { get; set; }      // כתובת מהטופס
//        public DateTime ScheduledDate { get; set; } // תאריך הגעה מבוקש
//        public string Description { get; set; }  // הערות שהלקוח הוסיף
//    }
//}


using System;

namespace FixUp.Service.Dto
{
    public class RequestCreateDto
    {
        public int ClientId { get; set; } // המזהה של הלקוח ששולח
        public int ProfessionalId { get; set; } // המזהה של בעל המקצוע
        public int CategoryId { get; set; } // המזהה של הקטגוריה
        public string Subject { get; set; } // "תיקון מזגן"
        public string Description { get; set; } // "המזגן מוציא אוויר חם"
        public string Address { get; set; } // כתובת הלקוח
        public DateTime ScheduledDate { get; set; } // תאריך ושעה מתוזמנים
        public string ImageUrl { get; set; } // התמונה שהוא צירף
    }
}

