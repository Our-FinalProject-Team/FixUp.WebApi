using FixUp.Service.Dto;
using System.ComponentModel.DataAnnotations;

public class ProfessionalDto : UserDto
{
    [Required(ErrorMessage = "תחום התמחות הוא שדה חובה")]
    public string Specialty { get; set; }

    public double AverageRating { get; set; }

    public int TotalReviews { get; set; }

    public int CategoryId => Specialty switch
    {
        "מתקין מערכות אבטחה" => 1,
        "טכנאי מזגנים" => 2,
        "צבעי" => 3,
        "אינסטלטור" => 4,
        "חשמלאי" => 5,
        "איש תחזוקה/הידמן" => 6,
        "ניקיון" => 7,
        "טכנאי מוצרי חשמל" => 8,
        "מנעולן" => 9,
        "גנן" => 10,
        "רצף/מתקין חיפויים" => 11,
        "שיפוצניק/קבלן בניה" => 12,
        _ => 0
    };

    [Range(0, 1000, ErrorMessage = "מחיר שעתי חייב להיות בין 0 ל-1000")]
    public double BaseHourlyRate { get; set; }

    [Range(0, 500, ErrorMessage = "דמי ביקור חייבים להיות בין 0 ל-500")]
    public double CallOutFee { get; set; }
}