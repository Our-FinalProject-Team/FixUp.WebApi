using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixUpSolution.Models
{
    
   
        public class Professional : User
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Specialty { get; set; }
            public decimal PricePerHour { get; set; }
       
        }
}
