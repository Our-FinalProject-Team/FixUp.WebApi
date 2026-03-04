using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixUp.Service.Dto
{
    public class RequestDisplayDto
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public string ProfessionalName { get; set; } // שליפה נוחה מה-DB במקום רק ID
        public DateTime CreatedAt { get; set; }
    }
}
