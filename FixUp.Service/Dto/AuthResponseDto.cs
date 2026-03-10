using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FixUp.Service.Dto
{
    public class AuthResponseDto
    {
        // אנחנו משתמשים ב-object כדי שזה יתאים גם ללקוח וגם לבעל מקצוע
        public object User { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}

