using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace FixUp.Service.Dto
{
    public class RequestCreateDto
    {
        public int ClientId { get; set; }        // מזהה הלקוח המחובר
        public int ProfessionalId { get; set; }  // המזהה של איש המקצוע שנבחר
        public int CategoryId { get; set; }      // הוספנו את זה! המזהה של הקטגוריה (1, 2, 3...)
        public string Address { get; set; }      // כתובת מהטופס
        public DateTime ScheduledDate { get; set; } // תאריך הגעה מבוקש
        public string Description { get; set; }  // הערות שהלקוח הוסיף
    }
}
