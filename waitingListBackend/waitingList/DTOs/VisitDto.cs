namespace waitingList.DTOs
{
    public sealed record VisitDto(
        Guid visitId,
        string accNumber,
        string fullName,
        string cellNumber,
        int centreId,
        string centreName,
        DateOnly visitDate,
        string status,
        DateTimeOffset createdAt,
        string qrToken,
        DateTimeOffset qrGeneratedAt);
}
