namespace waitingList.Models
{
    public class UnitCentre
    {
        public int centreId { get; set; }
        public required string externalCentreId { get; set; }
        public required string centreName { get; set; }
        public required string address { get; set; }
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
        public int allowedRadiusMetres { get; set; } = 150;
        public bool isActive { get; set; } = true;

        // Remembers when this centre was last updated from the external API.
        public DateTimeOffset? lastSyncedAt { get; set; }

        public ICollection<Visit> visits { get; set; } = [];
    }
}
