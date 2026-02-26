using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly IReviewRepository _reviewRepo;

    public ReviewsController(IReviewRepository reviewRepo)
    {
        _reviewRepo = reviewRepo;
    }

    [HttpGet("professional/{profId}")]
    public async Task<ActionResult<IEnumerable<Review>>> GetByProfessional(int profId)
    {
        var reviews = await _reviewRepo.GetReviewsByProfessionalIdAsync(profId);
        return Ok(reviews);
    }

    [HttpPost]
    public async Task<IActionResult> PostReview(Review review)
    {
        await _reviewRepo.AddReviewAsync(review);
        return Ok(review);
    }
}