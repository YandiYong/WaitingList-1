using Microsoft.EntityFrameworkCore;
using waitingList.Data;
using waitingList.DTOs;

namespace waitingList.Services
{
    public sealed class CentreService(WaitingListDbContext dbContext) : ICentreService
    {
        public async Task<IReadOnlyList<CentreDto>> getActiveCentresAsync(
            CancellationToken cancellationToken = default)
        {
            return await dbContext.unitCentres
                .AsNoTracking()
                .Where(centre => centre.isActive)
                .OrderBy(centre => centre.centreName)
                .Select(centre => new CentreDto(
                    centre.centreId,
                    centre.externalCentreId,
                    centre.centreName,
                    centre.address,
                    centre.latitude,
                    centre.longitude,
                    centre.allowedRadiusMetres))
                .ToListAsync(cancellationToken);
        }
    }
}
