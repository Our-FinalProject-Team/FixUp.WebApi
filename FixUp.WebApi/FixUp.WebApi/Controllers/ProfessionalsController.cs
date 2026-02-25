using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ProfessionalsController : ControllerBase
{
    private readonly IProfessionalRepository _profRepo;

    public ProfessionalsController(IProfessionalRepository profRepo)
    {
        _profRepo = profRepo;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Professional>>> GetAll()
    {
        return Ok(await _profRepo.GetAllProfessionalsAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Professional>> GetById(int id)
    {
        var prof = await _profRepo.GetProfessionalByIdAsync(id);
        if (prof == null) return NotFound();
        return Ok(prof);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfessional(int id, Professional professional)
    {
        if (id != professional.Id) return BadRequest();
        await _profRepo.UpdateProfessionalAsync(professional);
        return NoContent();
    }
}