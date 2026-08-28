using System.ComponentModel.DataAnnotations;

namespace waitingList.DTOs
{
    public sealed class CheckInRequestDto
    {
        [Required]
        [StringLength(100)]
        public required string qrToken { get; set; }

        [Range(-90, 90)]
        public double latitude { get; set; }

        [Range(-180, 180)]
        public double longitude { get; set; }
    }
}
