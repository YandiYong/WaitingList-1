using Microsoft.EntityFrameworkCore;
using waitingList.Data;
using waitingList.DTOs;
using waitingList.Models;

namespace waitingList.Services
{
    public sealed class VisitService(
        WaitingListDbContext dbContext,
        TimeProvider timeProvider) : IVisitService
    {
        public async Task<IReadOnlyList<VisitDto>> getVisitsAsync(
            CancellationToken cancellationToken = default)
        {
            return await dbContext.visits
                .AsNoTracking()
                .OrderByDescending(visit => visit.createdAt)
                .Select(visit => new VisitDto(
                    visit.visitId,
                    visit.client.accNumber,
                    visit.client.fullName,
                    visit.client.cellNumber,
                    visit.centreId,
                    visit.centre.centreName,
                    visit.visitDate,
                    visit.status.ToString(),
                    visit.createdAt))
                .ToListAsync(cancellationToken);
        }

        public async Task<VisitDto> createVisitAsync(
            CreateVisitRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var centre = await dbContext.unitCentres
                .SingleOrDefaultAsync(
                    item => item.centreId == request.centreId && item.isActive,
                    cancellationToken)
                ?? throw new KeyNotFoundException("The selected centre does not exist or is inactive.");

            var accNumber = request.accNumber.Trim();
            var fullName = request.fullName.Trim();
            var cellNumber = request.cellNumber.Trim();
            var localNow = timeProvider.GetLocalNow();

            var client = await dbContext.clients
                .SingleOrDefaultAsync(
                    item => item.accNumber == accNumber,
                    cancellationToken);

            if (client is null)
            {
                client = new Client
                {
                    accNumber = accNumber,
                    fullName = fullName,
                    cellNumber = cellNumber,
                    createdAt = localNow
                };

                dbContext.clients.Add(client);
            }
            else
            {
                // Keep the client's latest contact details.
                client.fullName = fullName;
                client.cellNumber = cellNumber;
                client.updatedAt = localNow;
            }

            var duplicateExists = await dbContext.visits.AnyAsync(
                visit => visit.clientId == client.clientId &&
                         visit.centreId == request.centreId &&
                         visit.visitDate == request.visitDate,
                cancellationToken);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    "This client already has a visit at the selected centre on this date.");
            }

            var visit = new Visit
            {
                client = client,
                centre = centre,
                centreId = centre.centreId,
                visitDate = request.visitDate,
                createdAt = localNow
            };

            dbContext.visits.Add(visit);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new VisitDto(
                visit.visitId,
                client.accNumber,
                client.fullName,
                client.cellNumber,
                centre.centreId,
                centre.centreName,
                visit.visitDate,
                visit.status.ToString(),
                visit.createdAt);
        }
    }
}
