
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using System.Security.Claims;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly RaceDayDbContext _context;

    public EventsController(RaceDayDbContext context)
    {
        _context = context;
    }

    // GET: api/Events
    // Authenticated users can view events.
    [HttpGet]
    public async Task<IActionResult> GetEvents()
    {
        var events = await _context.Events
            .AsNoTracking()
            .Select(e => new
            {
                e.EventId,
                e.EventName,
                e.EventDate,
                e.Location,
                e.Description,
                e.Status,
                e.OrganiserId
            })
            .ToListAsync();

        return Ok(events);
    }

    // POST: api/Events
    // Only Organisers can create events.
    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> CreateEvent(EventRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EventName) ||
            string.IsNullOrWhiteSpace(request.Location) ||
            string.IsNullOrWhiteSpace(request.Status))
        {
            return BadRequest(new
            {
                message = "Event name, location and status are required."
            });
        }

        if (request.EventDate < DateOnly.FromDateTime(DateTime.Today))
        {
            return BadRequest(new
            {
                message = "The event date cannot be in the past."
            });
        }

        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var organiser = await _context.Organisers
            .FirstOrDefaultAsync(o => o.UserId == userId);

        if (organiser == null)
        {
            return Forbid();
        }

        var newEvent = new Event
        {
            OrganiserId = organiser.OrganiserId,
            EventName = request.EventName.Trim(),
            EventDate = request.EventDate,
            Location = request.Location.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim(),
            Status = request.Status.Trim()
        };

        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEvents),
            new { id = newEvent.EventId },
            new
            {
                message = "Event created successfully.",
                newEvent.EventId,
                newEvent.EventName,
                newEvent.EventDate,
                newEvent.Location,
                newEvent.Description,
                newEvent.Status,
                newEvent.OrganiserId
            });
    }
}

public class EventRequest
{
    public string EventName { get; set; } = string.Empty;

    public DateOnly EventDate { get; set; }

    public string Location { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Status { get; set; } = "Upcoming";
}
