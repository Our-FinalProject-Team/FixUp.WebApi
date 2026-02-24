using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixUp.Repository.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Stars { get; set; } // 1-5
        public string Content { get; set; }
        public int ProfessionalId { get; set; } // מי בעל המקצוע שדורג
        public int ClientId { get; set; } // מי הלקוח שדירג

    }
}
