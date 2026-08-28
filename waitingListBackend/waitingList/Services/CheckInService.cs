using System.Data;
using Microsoft.EntityFrameworkCore;
using waitingList.Data;
using waitingList.DTOs;
using waitingList.Enums;
using waitingList.Models;

namespace waitingList.Services
{
    public sealed class CheckInService(
        WaitingListDbContext dbContext,
        TimeProvider timeProvider) : ICheckInService
    {
        private const double earthRadiusMetres = 6_371_000;

        public async Task<CheckInResultDto> getScanResultAsync(
            string qrToken,
            CancellationToken cancellationToken = default)
        {
            var visit = await findVisitAsync(qrToken, cancellationToken);
            validateVisitDateAndStatus(visit);

            if (visit.queueEntry is not null)
            {
                return await buildQueueResultAsync(visit, cancellationToken);
            }

            return new CheckInResultDto(
                true,
                "QR code scanned successfully.",
                visit.centre.centreName,
                visit.visitDate,
                visit.status.ToString(),
                null,
                null,
                null,
                null);
        }

        public async Task<CheckInResultDto> checkInAsync(
            CheckInRequestDto request,
            CancellationToken cancellationToken = default)
        {
            // Serializable prevents two visitors from receiving the same queue number.
            await using var transaction = await dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);

            var visit = await findVisitAsync(request.qrToken, cancellationToken);
            validateVisitDateAndStatus(visit);

            // Re-scanning an already checked-in appointment returns its existing place.
            if (visit.queueEntry is not null)
            {
                var existingResult = await buildQueueResultAsync(visit, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return existingResult;
            }

            var distanceMetres = calculateDistanceMetres(
                request.latitude,
                request.longitude,
                (double)visit.centre.latitude,
                (double)visit.centre.longitude);

            if (distanceMetres > visit.centre.allowedRadiusMetres)
            {
                throw new ArgumentException(
                    $"You must be within {visit.centre.allowedRadiusMetres} metres of {visit.centre.centreName} to check in.");
            }

            var localNow = timeProvider.GetLocalNow();
            var localDate = DateOnly.FromDateTime(localNow.DateTime);

            var lastQueueNumber = await dbContext.queueEntries
                .Where(entry => entry.centreId == visit.centreId &&
                                entry.queueDate == localDate)
                .MaxAsync(entry => (int?)entry.queueNumber, cancellationToken)
                ?? 0;

            var queueEntry = new QueueEntry
            {
                visit = visit,
                visitId = visit.visitId,
                centre = visit.centre,
                centreId = visit.centreId,
                queueDate = localDate,
                queueNumber = lastQueueNumber + 1,
                status = QueueStatus.Waiting,
                joinedAt = localNow
            };

            visit.status = VisitStatus.CheckedIn;
            visit.checkedInAt = localNow;
            visit.queueEntry = queueEntry;

            dbContext.queueEntries.Add(queueEntry);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return await buildQueueResultAsync(visit, cancellationToken);
        }

        private async Task<Visit> findVisitAsync(
            string qrToken,
            CancellationToken cancellationToken)
        {
            var cleanToken = qrToken.Trim();
            if (string.IsNullOrWhiteSpace(cleanToken))
            {
                throw new KeyNotFoundException("The QR code does not contain a valid appointment token.");
            }

            return await dbContext.visits
                .Include(visit => visit.centre)
                .Include(visit => visit.queueEntry)
                .SingleOrDefaultAsync(
                    visit => visit.qrToken == cleanToken,
                    cancellationToken)
                ?? throw new KeyNotFoundException("This QR code is invalid or does not belong to an appointment.");
        }

        private void validateVisitDateAndStatus(Visit visit)
        {
            var localToday = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);

            if (visit.visitDate != localToday)
            {
                throw new InvalidOperationException(
                    $"This appointment is booked for {visit.visitDate:dd/MM/yyyy}. You can only check in on the appointment date.");
            }

            if (visit.status == VisitStatus.Cancelled)
            {
                throw new InvalidOperationException("This appointment has been cancelled.");
            }

            if (visit.status == VisitStatus.Completed)
            {
                throw new InvalidOperationException("This appointment has already been completed.");
            }
        }

        private async Task<CheckInResultDto> buildQueueResultAsync(
            Visit visit,
            CancellationToken cancellationToken)
        {
            var queueEntry = visit.queueEntry
                ?? throw new InvalidOperationException("The queue entry could not be found.");

            var peopleAhead = await dbContext.queueEntries.CountAsync(
                entry => entry.centreId == queueEntry.centreId &&
                         entry.queueDate == queueEntry.queueDate &&
                         entry.status == QueueStatus.Waiting &&
                         entry.queueNumber < queueEntry.queueNumber,
                cancellationToken);

            return new CheckInResultDto(
                true,
                "QR code scanned and check-in completed successfully.",
                visit.centre.centreName,
                visit.visitDate,
                visit.status.ToString(),
                $"A{queueEntry.queueNumber:D3}",
                queueEntry.status.ToString().ToUpperInvariant(),
                peopleAhead,
                queueEntry.joinedAt);
        }

        private static double calculateDistanceMetres(
            double visitorLatitude,
            double visitorLongitude,
            double centreLatitude,
            double centreLongitude)
        {
            var latitudeDifference = toRadians(centreLatitude - visitorLatitude);
            var longitudeDifference = toRadians(centreLongitude - visitorLongitude);

            var visitorLatitudeRadians = toRadians(visitorLatitude);
            var centreLatitudeRadians = toRadians(centreLatitude);

            var haversine = Math.Pow(Math.Sin(latitudeDifference / 2), 2) +
                            Math.Cos(visitorLatitudeRadians) *
                            Math.Cos(centreLatitudeRadians) *
                            Math.Pow(Math.Sin(longitudeDifference / 2), 2);

            return earthRadiusMetres * 2 * Math.Atan2(
                Math.Sqrt(haversine),
                Math.Sqrt(1 - haversine));
        }

        private static double toRadians(double degrees) => degrees * Math.PI / 180;
    }
}
