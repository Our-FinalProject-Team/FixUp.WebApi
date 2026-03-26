using System.ComponentModel.DataAnnotations;

namespace FixUp.Service.Dto
{
    public class UserDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "שם מלא הוא שדה חובה")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "שם חייב להיות בין 2 ל-50 תווים")]
        [RegularExpression(@"^[a-zA-Zא-ת\s]+$", ErrorMessage = "שם יכול להכיל אותיות ורווחים בלבד")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "אימייל הוא שדה חובה")]
        [EmailAddress(ErrorMessage = "פורמט אימייל לא תקין")]   
        public string Email { get; set; }

        [Required(ErrorMessage = "מספר טלפון הוא חובה")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "טלפון חייב להכיל בדיוק 10 ספרות")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "כתובת היא שדה חובה")]
        public string Address { get; set; }
       
    } 
}