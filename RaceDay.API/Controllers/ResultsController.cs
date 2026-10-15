
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
public class ResultsController : ControllerBase
{
    private readonly RaceDayDbContext _context;

    public ResultsController(RaceDayDbContext context)
    {
        _context = context;
    }

    // GET: api/Results
    // Participants see their own results; organisers see all results.
    [HttpGet]
    public async Task<IActionResult> GetResults()
    {
        var userIdValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var query = _context.Results
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

            query = query.Where(r =>
                r.Enrolment.ParticipantId == participant.ParticipantId);
        }
        else if (!User.IsInRole("Organiser"))
        {
            return Forbid();
        }

        var results = await query
            .Select(r => new
            {
                r.ResultId,
                r.EnrolmentId,
                r.FinishTime,
                r.Position,
                r.ResultStatus
            })
            .ToListAsync();

        return Ok(results);
    }

    // GET: api/Results/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetResult(int id)
    {
        var userIdValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var result = await _context.Results
            .AsNoTracking()
            .Where(r => r.ResultId == id)
            .Select(r => new
            {
                r.ResultId,
                r.EnrolmentId,
                r.FinishTime,
                r.Position,
                r.ResultStatus,
                ParticipantUserId = r.Enrolment.Participant.UserId
            })
            .FirstOrDefaultAsync();

        if (result == null)
        {
            return NotFound(new
            {
                message = "Result not found."
            });
        }

        if (User.IsInRole("Participant") &&
            result.ParticipantUserId != userId)
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
            result.ResultId,
            result.EnrolmentId,
            result.FinishTime,
            result.Position,
            result.ResultStatus
        });
    }

    // POST: api/Results
    // Only organisers can record results.
    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> CreateResult(
        ResultRequest request)
    {
        if (request.EnrolmentId <= 0)
        {
            return BadRequest(new
            {
                message = "A valid enrolment ID is required."
            });
        }

        if (request.Position <= 0)
        {
            return BadRequest(new
            {
                message = "Position must be greater than zero."
            });
        }

        if (string.IsNullOrWhiteSpace(request.ResultStatus))
        {
            return BadRequest(new
            {
                message = "Result status is required."
            });
        }

        var enrolment = await _context.Enrolments
            .FirstOrDefaultAsync(e =>
                e.EnrolmentId == request.EnrolmentId);

        if (enrolment == null)
        {
            return BadRequest(new
            {
                message = "The specified enrolment does not exist."
            });
        }

        var existingResult = await _context.Results
            .AnyAsync(r => r.EnrolmentId == request.EnrolmentId);

        if (existingResult)
        {
            return Conflict(new
            {
                message = "A result already exists for this enrolment."
            });
        }

        var result = new Result
        {
            EnrolmentId = request.EnrolmentId,
            FinishTime = request.FinishTime,
            Position = request.Position,
            ResultStatus = request.ResultStatus.Trim()
        };

        _context.Results.Add(result);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetResult),
            new { id = result.ResultId },
            new
            {
                message = "Result recorded successfully.",
                result.ResultId,
                result.EnrolmentId,
                result.FinishTime,
                result.Position,
                result.ResultStatus
            });
    }
}

public class ResultRequest
{
    public int EnrolmentId { get; set; }

    public TimeOnly FinishTime { get; set; }

    public int Position { get; set; }

    public string ResultStatus { get; set; } = "Finished";
}
