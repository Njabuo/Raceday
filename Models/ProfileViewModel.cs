using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class ProfileViewModel
    {
        public int UserId { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public string EmergencyContact { get; set; } = string.Empty;

        [Required]
        public string EmergencyPhone { get; set; } = string.Empty;
    }
}