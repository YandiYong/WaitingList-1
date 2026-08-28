using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using waitingList.Models;

namespace waitingList.Data
{
    public sealed class CentreDataSeeder(
        WaitingListDbContext dbContext,
        IWebHostEnvironment environment,
        TimeProvider timeProvider)
    {
        public async Task seedAsync(CancellationToken cancellationToken = default)
        {
            var filePath = Path.Combine(
                environment.ContentRootPath,
                "Data",
                "centres.json");

            await using var stream = File.OpenRead(filePath);
            var testCentres = await JsonSerializer.DeserializeAsync<List<UnitCentre>>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken) ?? [];

            var localNow = timeProvider.GetLocalNow();

            foreach (var testCentre in testCentres)
            {
                var centre = await dbContext.unitCentres.SingleOrDefaultAsync(
                    item => item.externalCentreId == testCentre.externalCentreId,
                    cancellationToken);

                if (centre is null)
                {
                    testCentre.lastSyncedAt = localNow;
                    dbContext.unitCentres.Add(testCentre);
                    continue;
                }

                // The JSON file behaves like the future external centre API.
                centre.centreName = testCentre.centreName;
                centre.address = testCentre.address;
                centre.latitude = testCentre.latitude;
                centre.longitude = testCentre.longitude;
                centre.allowedRadiusMetres = testCentre.allowedRadiusMetres;
                centre.isActive = testCentre.isActive;
                centre.lastSyncedAt = localNow;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
