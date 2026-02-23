using FixUp.Repository.Interfaces;
using FixUpSolution.Models;
using Microsoft.AspNetCore.Mvc;

namespace FixUp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // הכתובת תהיה api/users
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        // כאן אנחנו מזריקים את האינטרפייס (ולא את הריפוזיטורי ישירות!)
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // 1. קבלת כל המשתמשים
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users); // מחזיר קוד 200 עם רשימת המשתמשים
        }

        // 2. קבלת משתמש ספציפי לפי ID
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("משתמש לא נמצא"); // קוד 404
            }
            return Ok(user);
        }

        // 3. יצירת משתמש חדש
        [HttpPost]
        public async Task<ActionResult> Create(User user)
        {
            await _userRepository.AddUserAsync(user);
            // מחזיר קוד 201 ומראה איפה אפשר למצוא את המשאב החדש
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        // 4. עדכון משתמש
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest("ה-ID לא תואם");
            }

            await _userRepository.UpdateUserAsync(user);
            return NoContent(); // קוד 204 - הצלחתי ואין לי מה להוסיף
        }

        // 5. מחיקת משתמש
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _userRepository.DeleteUserAsync(id);
            return Ok("המשתמש נמחק בהצלחה");
        }
    }
}