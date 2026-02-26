using Microsoft.AspNetCore.Mvc;
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;

namespace FixUp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepo;

        public ClientsController(IClientRepository clientRepo)
        {
            _clientRepo = clientRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetAll() => Ok(await _clientRepo.GetAllClientsAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetById(int id)
        {
            var client = await _clientRepo.GetClientByIdAsync(id);
            return client == null ? NotFound() : Ok(client);
        }
    }
}
