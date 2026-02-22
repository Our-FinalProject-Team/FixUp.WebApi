using Microsoft.AspNetCore.Mvc;
using FixUpSolution.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FixUp.WebApi_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessionalsController : ControllerBase
    {
        private static List<Professional> _professionals = new List<Professional>();
        [HttpGet]
        public IEnumerable<Professional> Get()
        {
            return _professionals;
        }

        // GET api/<ProfessionalsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody] Professional value)
        {
            _professionals.Add(value);
        }
        // PUT api/<ProfessionalsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProfessionalsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
