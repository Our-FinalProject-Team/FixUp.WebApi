using FixUp.Service.Interfases;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixUp.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        // הבנאי (Constructor) שמקבל את הקונפיגורציה מה-appsettings
        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // 1. משיכת הנתונים מה-appsettings.json
            var fromEmail = _config["EmailSettings:Email"];
            var password = _config["EmailSettings:Password"];
            var host = _config["EmailSettings:Host"];
            var port = int.Parse(_config["EmailSettings:Port"]);

            var email = new MimeMessage(); // הורדתי את ה-toEmail מהסוגריים כי זה לא תקין שם
            email.From.Add(new MailboxAddress("FixUp System", fromEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            // 3. תהליך השליחה
            using var smtp = new SmtpClient();

            // שימוש במשתנים שמשכנו מהקונפיגורציה
            await smtp.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);

            // כאן המערכת תשתמש ב-16 התווים שהגדרת ב-JSON
            await smtp.AuthenticateAsync(fromEmail, password);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
