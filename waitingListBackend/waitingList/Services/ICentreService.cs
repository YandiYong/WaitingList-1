using waitingList.DTOs;

namespace waitingList.Services
{
    public interface ICentreService
    {
        Task<IReadOnlyList<CentreDto>> getActiveCentresAsync(
            CancellationToken cancellationToken = default);
    }
}
