using waitingList.DTOs;

namespace waitingList.Services
{
    public interface IVisitService
    {
        Task<IReadOnlyList<VisitDto>> getVisitsAsync(
            CancellationToken cancellationToken = default);

        Task<VisitDto> createVisitAsync(
            CreateVisitRequestDto request,
            CancellationToken cancellationToken = default);
    }
}
