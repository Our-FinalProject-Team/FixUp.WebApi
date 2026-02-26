using FixUp.Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ProfessionalDto : UserDto
{
    public string Specialty { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public bool IsActiveNow { get; set; }
    public double CurrentLat { get; set; }
    public double CurrentLng { get; set; }
    public double BaseHourlyRate { get; set; }
    public double CallOutFee { get; set; }
}