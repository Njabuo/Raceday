namespace RaceDay.MVC.Models
{
    public class OrganiserEnrolmentViewModel
    {
        public int EnrolmentId { get; set; }

        public int ParticipantId { get; set; }

        public int CategoryId { get; set; }

        public DateOnly EnrolmentDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string RaceNumber { get; set; } = string.Empty;
    }
}