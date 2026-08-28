using waitingList.Enums;

namespace waitingList.Models
{
    // A queue entry is created only after a visitor checks in successfully.
    public class QueueEntry
    {
        public Guid queueEntryId { get; set; } = Guid.NewGuid();

        public Guid visitId { get; set; }
        public Visit visit { get; set; } = null!;

        public int centreId { get; set; }
        public UnitCentre centre { get; set; } = null!;

        public DateOnly queueDate { get; set; }
        public int queueNumber { get; set; }
        public QueueStatus status { get; set; } = QueueStatus.Waiting;

        // This timestamp comes from the computer's local clock.
        public DateTimeOffset joinedAt { get; set; }
    }
}
