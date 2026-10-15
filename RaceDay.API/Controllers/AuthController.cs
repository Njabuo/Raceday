
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RaceDayDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AuthController(RaceDayDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (request.DateOfBirth == default ||
            request.DateOfBirth >= DateOnly.FromDateTime(DateTime.Today))
        {
            return BadRequest(new
            {
                message = "Please provide a valid date of birth in the past."
            });
        }

        bool emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email);

        if (emailExists)
        {
            return Conflict(new
            {
                message = "An account with this email already exists."
            });
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var user = new User
            {
                FullName = request.FullName.Trim(),
                Email = request.Email.Trim(),
                Role = "Participant",
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var participant = new Participant
            {
                UserId = user.UserId,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender.Trim(),
                EmergencyContact = request.EmergencyContact.Trim(),
                EmergencyPhone = request.EmergencyPhone.Trim()
            };

            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return Created($"/api/auth/register/{user.UserId}", new
            {
                message = "Participant registered successfully.",
                userId = user.UserId,
                fullName = user.FullName,
                email = user.Email,
                role = user.Role
            });
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();

            return Conflict(new
            {
                message = "The account could not be saved. Check the submitted details."
            });
        }
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new
            {
                message = "Email and password are required."
            });
        }

        var email = request.Email.Trim();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var passwordResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var jwtKey = HttpContext.RequestServices
            .GetRequiredService<IConfiguration>()["Jwt:Key"]!;

        var jwtIssuer = HttpContext.RequestServices
            .GetRequiredService<IConfiguration>()["Jwt:Issuer"]!;

        var jwtAudience = HttpContext.RequestServices
            .GetRequiredService<IConfiguration>()["Jwt:Audience"]!;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));

        var credentials = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return Ok(new
        {
            message = "Login successful.",
            userId = user.UserId,
            fullName = user.FullName,
            email = user.Email,
            role = user.Role,
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }

    [Authorize]
    [HttpGet("profile")]
    public IActionResult Profile()
    {
        return Ok(new
        {
            message = "You are authenticated.",
            userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            name = User.Identity?.Name,
            role = User.FindFirst(ClaimTypes.Role)?.Value
        });
    }

    [Authorize(Roles = "Participant")]
    [HttpGet("participant-area")]
    public IActionResult ParticipantArea()
    {
        return Ok(new
        {
            message = "Welcome to the Participant area.",
            role = User.FindFirst(ClaimTypes.Role)?.Value
        });
    }

    [Authorize(Roles = "Organiser")]
    [HttpGet("organiser-area")]
    public IActionResult OrganiserArea()
    {
        return Ok(new
        {
            message = "Welcome to the Organiser area.",
            role = User.FindFirst(ClaimTypes.Role)?.Value
        });
    }

}
