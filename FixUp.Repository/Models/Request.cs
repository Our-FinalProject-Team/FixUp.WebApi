using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace FixUp.Repository.Models
{
  
    public class Request
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int? ProfessionalId { get; set; } // יכול להיות null בהתחלה
        public string Subject { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }     
    }
}

