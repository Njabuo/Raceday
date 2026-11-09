
namespace RaceDay.MVC.Models
{
    public class EventViewModel
    {
        public int EventId { get; set; }

        public string EventName { get; set; } = string.Empty;
        public string? BannerImageBlobName { get; set; }

        public DateOnly EventDate { get; set; }

        public string Location { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Status { get; set; } = string.Empty;

        public int OrganiserId { get; set; }
    }
}
