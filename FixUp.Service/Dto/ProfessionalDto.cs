using FixUp.Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public class ProfessionalDto : UserDto
{
    public string Specialty { get; set; } // תחום התמחות (חשמל, אינסטלציה וכו')
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public double BaseHourlyRate { get; set; }
    public double CallOutFee { get; set; }
}