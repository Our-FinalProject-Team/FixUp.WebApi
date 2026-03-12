using FixUp.Service.Dto;
using System.ComponentModel.DataAnnotations;

public class ProfessionalDto : UserDto
{
    [Required(ErrorMessage = "תחום התמחות הוא שדה חובה")]
    public string Specialty { get; set; }

    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }

    [Range(0, 1000, ErrorMessage = "מחיר שעתי חייב להיות בין 0 ל-1000")]
    public double BaseHourlyRate { get; set; }

    [Range(0, 500, ErrorMessage = "דמי ביקור חייבים להיות בין 0 ל-500")]
    public double CallOutFee { get; set; }
}