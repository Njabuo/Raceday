using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class CreateEventViewModel
    {
        [Required(ErrorMessage = "Please enter the event name.")]
        public string EventName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateOnly EventDate { get; set; }

        [Required(ErrorMessage = "Please enter the event location.")]
        public string Location { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public string Status { get; set; } = "Upcoming";
    }
}