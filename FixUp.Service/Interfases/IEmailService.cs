using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixUp.Service.Interfases
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toemail,string subject,string body);
    }
}
