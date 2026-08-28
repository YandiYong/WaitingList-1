namespace waitingList.DTOs
{
    public sealed record CheckInResultDto(
        bool scanSuccessful,
        string message,
        string centreName,
        DateOnly visitDate,
        string appointmentStatus,
        string? queueNumber,
        string? queueStatus,
        int? peopleAhead,
        DateTimeOffset? joinedAt);
}
