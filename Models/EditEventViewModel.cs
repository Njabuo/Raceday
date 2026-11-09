
using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class EditEventViewModel
    {
        public int EventId { get; set; }

        [Required(ErrorMessage = "Event name is required.")]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public DateOnly EventDate { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public string Status { get; set; } = "Upcoming";
    }
}
