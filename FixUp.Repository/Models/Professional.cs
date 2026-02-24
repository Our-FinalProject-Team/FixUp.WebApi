using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixUp.Repository.Models
{


    public class Professional : User
    {
        public string Specialty { get; set; } // תחום התמחות (חשמל, אינסטלציה, בייביסיטר)
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }

        // שדות ה-SOS הקריטיים:
        public bool IsActiveNow { get; set; } // האם הוא זמין לקבל SOS ברגע זה?
        public double CurrentLat { get; set; } // קואורדינטות בזמן אמת
        public double CurrentLng { get; set; }

        public List<Review> Reviews { get; set; } // רשימת חוות הדעת עליו
        public double BaseHourlyRate { get; set; } // מחיר לשעה בשגרה
        public double CallOutFee { get; set; }     // מחיר רק על הגעה (ביקור)


    }
}
