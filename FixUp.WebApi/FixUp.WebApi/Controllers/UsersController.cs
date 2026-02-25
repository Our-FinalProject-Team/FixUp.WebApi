using Microsoft.AspNetCore.Mvc;
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;

namespace FixUp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll() => Ok(await _userRepo.GetAllUsersAsync());

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userRepo.DeleteUserAsync(id);
            return NoContent();
        }
    }
}