using waitingList.Enums;

namespace waitingList.Models
{
    public class Client
    {
        public Guid clientId { get; set; } = Guid.NewGuid();
        public required string accNumber { get; set; }
        public required string fullName { get; set; }
        public required string cellNumber { get; set; }
        public ClientStatus status { get; set; } = ClientStatus.Active;

        // Services assign timestamps from the computer's local clock.
        public DateTimeOffset createdAt { get; set; }
        public DateTimeOffset? updatedAt { get; set; }

        public ICollection<Visit> visits { get; set; } = [];
    }
}
