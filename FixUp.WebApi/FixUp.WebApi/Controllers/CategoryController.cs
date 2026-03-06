using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;

using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepo;

    public CategoriesController(ICategoryRepository categoryRepo)
    {
        _categoryRepo = categoryRepo;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        return Ok(await _categoryRepo.GetAllCategoriesAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Add(Category category)
    {
        await _categoryRepo.AddCategoryAsync(category);
        return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
    }
}