using waitingList.DTOs;

namespace waitingList.Services
{
    public interface ICheckInService
    {
        Task<CheckInResultDto> getScanResultAsync(
            string qrToken,
            CancellationToken cancellationToken = default);

        Task<CheckInResultDto> checkInAsync(
            CheckInRequestDto request,
            CancellationToken cancellationToken = default);
    }
}
