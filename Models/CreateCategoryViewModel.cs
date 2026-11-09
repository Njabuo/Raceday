using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class CreateCategoryViewModel
    {
        [Required]
        public int EventId { get; set; }

        [Required(ErrorMessage = "Enter the category name.")]
        public string CategoryName { get; set; } = string.Empty;

        [Range(0.1, 100, ErrorMessage = "Distance must be greater than zero.")]
        public decimal DistanceKm { get; set; }

        [Range(1, 100000, ErrorMessage = "Maximum participants must be at least 1.")]
        public int MaximumParticipants { get; set; }
    }
}