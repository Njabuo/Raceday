
using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class CreateResultViewModel
    {
        [Required]
        public int EnrolmentId { get; set; }

        [Required]
        public TimeOnly FinishTime { get; set; }

        [Range(1, 100000, ErrorMessage = "Position must be greater than zero.")]
        public int Position { get; set; }

        [Required]
        public string ResultStatus { get; set; } = "Finished";
    }
}
