
namespace RaceDay.MVC.Models
{
    public class ResultViewModel
    {
        public int ResultId { get; set; }

        public int EnrolmentId { get; set; }

        public TimeOnly FinishTime { get; set; }

        public int Position { get; set; }

        public string ResultStatus { get; set; } = string.Empty;
    }
}
