
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
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayDbContext _context;

    public EnrolmentsController(RaceDayDbContext context)
    {
        _context = context;
    }

    // GET: api/Enrolments
    // Participants see their own enrolments.
    // Organisers can see all enrolments.
    [HttpGet]
    public async Task<IActionResult> GetEnrolments()
    {
        var userIdValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var query = _context.Enrolments
            .AsNoTracking()
            .AsQueryable();

        if (User.IsInRole("Participant"))
        {
            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (participant == null)
            {
                return Forbid();
            }

            query = query.Where(
                e => e.ParticipantId == participant.ParticipantId);
        }
        else if (!User.IsInRole("Organiser"))
        {
            return Forbid();
        }

        var enrolments = await query
            .Select(e => new
            {
                e.EnrolmentId,
                e.ParticipantId,
                e.CategoryId,
                e.EnrolmentDate,
                e.Status,
                e.RaceNumber
            })
            .ToListAsync();

        return Ok(enrolments);
    }

    // GET: api/Enrolments/5
    // A participant can view their own enrolment.
    // An organiser can view any enrolment.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetEnrolment(int id)
    {
        var userIdValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var enrolment = await _context.Enrolments
            .AsNoTracking()
            .Where(e => e.EnrolmentId == id)
            .Select(e => new
            {
                e.EnrolmentId,
                e.ParticipantId,
                e.CategoryId,
                e.EnrolmentDate,
                e.Status,
                e.RaceNumber,
                ParticipantUserId = e.Participant.UserId
            })
            .FirstOrDefaultAsync();

        if (enrolment == null)
        {
            return NotFound(new
            {
                message = "Enrolment not found."
            });
        }

        if (User.IsInRole("Participant") &&
            enrolment.ParticipantUserId != userId)
        {
            return Forbid();
        }

        if (!User.IsInRole("Participant") &&
            !User.IsInRole("Organiser"))
        {
            return Forbid();
        }

        return Ok(new
        {
            enrolment.EnrolmentId,
            enrolment.ParticipantId,
            enrolment.CategoryId,
            enrolment.EnrolmentDate,
            enrolment.Status,
            enrolment.RaceNumber
        });
    }

    // POST: api/Enrolments
    // Participants can enrol themselves in a category.
    [HttpPost]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> CreateEnrolment(
        EnrolmentRequest request)
    {
        if (request.CategoryId <= 0)
        {
            return BadRequest(new
            {
                message = "A valid category ID is required."
            });
        }

        var userIdValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var participant = await _context.Participants
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (participant == null)
        {
            return Forbid();
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId);

        if (category == null)
        {
            return BadRequest(new
            {
                message = "The specified category does not exist."
            });
        }

        var existingEnrolment = await _context.Enrolments
            .AnyAsync(e =>
                e.ParticipantId == participant.ParticipantId &&
                e.CategoryId == request.CategoryId);

        if (existingEnrolment)
        {
            return Conflict(new
            {
                message = "You are already enrolled in this category."
            });
        }

        var enrolmentCount = await _context.Enrolments
            .CountAsync(e =>
                e.CategoryId == request.CategoryId &&
                e.Status != "Cancelled");

        if (enrolmentCount >= category.MaximumParticipants)
        {
            return Conflict(new
            {
                message = "This category has reached its maximum capacity."
            });
        }

        var enrolment = new Enrolment
        {
            ParticipantId = participant.ParticipantId,
            CategoryId = request.CategoryId,
            EnrolmentDate = DateOnly.FromDateTime(DateTime.Today),
            Status = "Pending",
            RaceNumber = "RD-" + Guid.NewGuid()
                .ToString("N")[..8].ToUpperInvariant()
        };

        _context.Enrolments.Add(enrolment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEnrolment),
            new { id = enrolment.EnrolmentId },
            new
            {
                message = "Enrolment created successfully.",
                enrolment.EnrolmentId,
                enrolment.ParticipantId,
                enrolment.CategoryId,
                enrolment.EnrolmentDate,
                enrolment.Status,
                enrolment.RaceNumber
            });
    }
}

public class EnrolmentRequest
{
    public int CategoryId { get; set; }
}
