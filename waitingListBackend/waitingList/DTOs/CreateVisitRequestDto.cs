using System.ComponentModel.DataAnnotations;

namespace waitingList.DTOs
{
    public sealed class CreateVisitRequestDto
    {
        [Required, StringLength(20, MinimumLength = 4)]
        public required string accNumber { get; set; }

        [Required, StringLength(100, MinimumLength = 2)]
        public required string fullName { get; set; }

        [Required, RegularExpression(@"^(?:\+27|0)[6-8][0-9]{8}$")]
        public required string cellNumber { get; set; }

        [Range(1, int.MaxValue)]
        public int centreId { get; set; }

        public DateOnly visitDate { get; set; }
    }
}
