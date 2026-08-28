using waitingList.Enums;

namespace waitingList.Models
{
    public class Visit
    {
        public Guid visitId { get; set; } = Guid.NewGuid();

        public Guid clientId { get; set; }
        public Client client { get; set; } = null!;

        public int centreId { get; set; }
        public UnitCentre centre { get; set; } = null!;

        public DateOnly visitDate { get; set; }
        public VisitStatus status { get; set; } = VisitStatus.Scheduled;

        // Services assign timestamps from the computer's local clock.
        public DateTimeOffset createdAt { get; set; }
        public DateTimeOffset? checkedInAt { get; set; }
        public DateTimeOffset? cancelledAt { get; set; }
    }
}
