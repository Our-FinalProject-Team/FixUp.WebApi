using FixUp.Repository.Models;
//using FixUpSolution.Models; // כדי להכיר את ה-Enums (TaskType, TaskStatus)
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskStatus = FixUp.Repository.Models.TaskStatus;

namespace FixUp.Service.Dto
{
    public class FixUpTaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        // שימוש ב-Enums שהגדרת במודל
        public TaskType Type { get; set; }
        public TaskStatus Status { get; set; }

        public DateTime? ScheduledDate { get; set; }
        public double PriceEstimate { get; set; }
        public double FinalPrice { get; set; }
        public string PriceNegotiationStatus { get; set; }

        // מזהים בלבד (IDs) במקום אובייקטים מלאים
        public int ClientId { get; set; }
        public int? ProfessionalId { get; set; }

        // מיקום המשימה
        public double LocationLat { get; set; }
        public double LocationLng { get; set; }
    }
}