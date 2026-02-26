using FixUp.Service.Interfaces;
using FixUp.Service.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FixUp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // 1. קבלת כל המשתמשים (משתמש בפונקציה הגנרית מה-Base)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // 2. קבלת משתמש לפי ID (משתמש בפונקציה הגנרית מה-Base)
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var userDto = await _userService.GetByIdAsync(id);
            if (userDto == null)
            {
                return NotFound("משתמש לא נמצא");
            }
            return Ok(userDto);
        }

        // 3. רישום משתמש חדש (פונקציה ספציפית ב-IUserService שמקבלת סיסמה)
        [HttpPost("register")]
        public async Task<ActionResult> Register(UserDto userDto, string password)
        {
            await _userService.AddAsync(userDto);
            return CreatedAtAction(nameof(GetById), new { id = userDto.Id }, userDto);
        }

        // 4. התחברות - Login (הפונקציה המיוחדת שהוספנו)
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(UserLoginDto loginDto)
        {
            var user = await _userService.Authenticate(loginDto);

            if (user == null)
            {
                return Unauthorized("שם משתמש או סיסמה שגויים");
            }

            return Ok(user);
        }

        // 5. עדכון משתמש (גנרי)
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UserDto userDto)
        {
            await _userService.UpdateAsync(id, userDto);
            return NoContent();
        }

        // 6. מחיקת משתמש (גנרי)
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }
        // יצירת משתמש חדש - שימוש ב-RegisterAsync
        [HttpPost("register")]
        public async Task<ActionResult> RegisterAsync(UserDto userDto, string password)
        {
            // הסרוויס יטפל בהצמדת הסיסמה למודל לפני השמירה
            await _userService.RegisterAsync(userDto, password);
            return CreatedAtAction(nameof(GetById), new { id = userDto.Id }, userDto);
        }
    }
}