using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace FixUp.Service.Interfaces
{
    public interface IAuthService
    {
        // הפונקציה מקבלת אימייל ותפקיד (Role) ומחזירה מחרוזת של טוקן חתום
        string GenerateJwtToken(string email, string role);
    }
}