
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs;

public class RegisterRequest
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public DateOnly DateOfBirth { get; set; }

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public string EmergencyContact { get; set; } = string.Empty;

    [Required]
    public string EmergencyPhone { get; set; } = string.Empty;
}
