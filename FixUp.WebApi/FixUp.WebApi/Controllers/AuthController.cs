using FixUp.Service.Dto;
using FixUp.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FixUp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IClientService _clientService;
        private readonly IProfessionalService _professionalService;

        public AuthController(IAuthService authService, IClientService clientService, IProfessionalService professionalService)
        {
            _authService = authService;
            _clientService = clientService;
            _professionalService = professionalService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            try
            {
                // בדיקה אם זה לקוח
                var clients = await _clientService.GetAllAsync();
                var client = clients.FirstOrDefault(c => c.Email == loginDto.Email);
                
                if (client != null)
                {
                    // בדיקת סיסמה פשוטה - במערכת אמיתית צריך הצפנה!
                    if (loginDto.Password == "123456") // זמני - במערכת אמיתית צריך להשוות ל-BCrypt.Verify
                    {
                        var token = _authService.GenerateJwtToken(client.Email, "Client");
                        return Ok(new { 
                            token, 
                            user = new { 
                                id = client.Id, 
                                email = client.Email, 
                                name = client.FullName,
                                role = "Client" 
                            } 
                        });
                    }
                }

                // בדיקה אם זה בעל מקצוע
                var professionals = await _professionalService.GetAllAsync();
                var professional = professionals.FirstOrDefault(p => p.Email == loginDto.Email);
                
                if (professional != null)
                {
                    // בדיקת סיסמה פשוטה - במערכת אמיתית צריך הצפנה!
                    if (loginDto.Password == "123456") // זמני - במערכת אמיתית צריך להשוות ל-BCrypt.Verify
                    {
                        var token = _authService.GenerateJwtToken(professional.Email, "Professional");
                        return Ok(new { 
                            token, 
                            user = new { 
                                id = professional.Id, 
                                email = professional.Email, 
                                name = professional.FullName,
                                role = "Professional" 
                            } 
                        });
                    }
                }

                return Unauthorized("אימייל או סיסמה לא נכונים");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"שגיאה בהתחברות: {ex.Message}");
            }
        }
    }
}
