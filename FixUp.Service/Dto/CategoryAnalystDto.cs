using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixUp.Service.Dto
{
    public class CategoryAnalystDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public double ConfidenceScore { get; set; }
    }
}
