using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixUp.Service.Dto
{
    public class RequestCreateDto
    {
        public int ClientId { get; set; } // המזהה של הלקוח ששולח
        public string Subject { get; set; } // "תיקון מזגן"
        public string Description { get; set; } // "המזגן מוציא אוויר חם"
        public string ImageUrl { get; set; } // התמונה שהוא צירף
    }
}
