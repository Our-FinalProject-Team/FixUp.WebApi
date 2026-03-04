using FixUp.Repository.Models;
//using FixUpSolution.Models; // כדי להכיר את ה-Enums (TaskType, TaskStatus)
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixUp.Repository.Models;
using System;

namespace FixUp.Service.Dto
{
    public class FixUpTaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
       // public TaskStatus Status { get; set; }
        public DateTime ScheduledDate { get; set; } // הורדתי את ה-? כי זה חובה
        public double PriceEstimate { get; set; }
        public double FinalPrice { get; set; }
        public string PriceNegotiationStatus { get; set; }

        public int ClientId { get; set; }
        public int? ProfessionalId { get; set; }
    }
}