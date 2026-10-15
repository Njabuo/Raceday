
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly RaceDayDbContext _context;

    public CategoriesController(RaceDayDbContext context)
    {
        _context = context;
    }

    // GET: api/Categories
    // Authenticated users can view categories.
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .Select(c => new
            {
                c.CategoryId,
                c.EventId,
                c.CategoryName,
                c.DistanceKm,
                c.MaximumParticipants
            })
            .ToListAsync();

        return Ok(categories);
    }

    // GET: api/Categories/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .Where(c => c.CategoryId == id)
            .Select(c => new
            {
                c.CategoryId,
                c.EventId,
                c.CategoryName,
                c.DistanceKm,
                c.MaximumParticipants
            })
            .FirstOrDefaultAsync();

        if (category == null)
        {
            return NotFound(new
            {
                message = "Category not found."
            });
        }

        return Ok(category);
    }

    // POST: api/Categories
    // Only Organisers can create categories.
    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> CreateCategory(
        CategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CategoryName))
        {
            return BadRequest(new
            {
                message = "Category name is required."
            });
        }

        if (request.DistanceKm <= 0 ||
            request.MaximumParticipants <= 0)
        {
            return BadRequest(new
            {
                message = "Distance and maximum participants must be greater than zero."
            });
        }

        var eventRecord = await _context.Events
            .FirstOrDefaultAsync(e => e.EventId == request.EventId);

        if (eventRecord == null)
        {
            return BadRequest(new
            {
                message = "The specified event does not exist."
            });
        }

        var category = new Category
        {
            EventId = request.EventId,
            CategoryName = request.CategoryName.Trim(),
            DistanceKm = request.DistanceKm,
            MaximumParticipants = request.MaximumParticipants
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCategory),
            new { id = category.CategoryId },
            new
            {
                message = "Category created successfully.",
                category.CategoryId,
                category.EventId,
                category.CategoryName,
                category.DistanceKm,
                category.MaximumParticipants
            });
    }
}

public class CategoryRequest
{
    public int EventId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public decimal DistanceKm { get; set; }

    public int MaximumParticipants { get; set; }
}
