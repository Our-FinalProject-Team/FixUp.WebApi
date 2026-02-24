using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixUp.Repository.Models
{
    public enum TaskType { Maintenance, SOS }
    public enum TaskStatus { Pending, InProgress, Completed, Cancelled }

    public class FixUpTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        // הגדרת סוג המשימה - כאן נכנס ה-SOS!
        public TaskType Type { get; set; }
        public TaskStatus Status { get; set; }

        public DateTime? ScheduledDate { get; set; } // null אם זה SOS מיידי
        public double PriceEstimate { get; set; }

        // קשרים
        public int ClientId { get; set; }
        public Client Client { get; set; }

        public int? ProfessionalId { get; set; }
        public Professional Professional { get; set; }

        // מיקום המשימה (לצורך חישוב מרחק ל-Professional הקרוב ביותר)
        public double LocationLat { get; set; }
        public double LocationLng { get; set; }
        public double FinalPrice { get; set; }     // המחיר שנסגר בסוף
        public string PriceNegotiationStatus { get; set; } // "Proposed", "Accepted", "Paid"

    }
}
