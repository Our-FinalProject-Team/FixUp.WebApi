using FixUp.Service.Dto;
using FixUp.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FixUp.Repository.Models;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FixUp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FixUpTasksController : ControllerBase
    {
        // GET: api/<FixUpTasksController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<FixUpTasksController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<FixUpTasksController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<FixUpTasksController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FixUpTasksController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}