using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FixUp.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FixUp.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJwtToken(string email, string role)
        {
            // יצירת רשימת ה-Claims שתצורף לטוקן
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role) // הוספת התפקיד לתוך הטוקן לזיהוי במערכת
            };

            // הגדרת מפתח ההצפנה מהקונפיגורציה
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // יצירת אובייקט הטוקן
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7), // הטוקן יהיה תקף ל-7 ימים
                signingCredentials: creds
            );

            // החזרת הטוקן כמחרוזת
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}