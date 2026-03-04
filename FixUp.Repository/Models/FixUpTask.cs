using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixUp.Repository.Models
{
    //public enum TaskType { Maintenance }
    public enum TaskStatus { Pending, InProgress, Completed, Cancelled }

    public class FixUpTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime ScheduledDate { get; set; } // חובה, לא Nullable
        public double PriceEstimate { get; set; }
        public double FinalPrice { get; set; }
        public string PriceNegotiationStatus { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }
        public int? ProfessionalId { get; set; }
        public Professional Professional { get; set; }
    }
}
