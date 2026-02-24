using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixUp.Repository.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } // "אינסטלציה", "חשמל"
        public string IconUrl { get; set; } // בשביל ה-UI ב-React
    }
}
