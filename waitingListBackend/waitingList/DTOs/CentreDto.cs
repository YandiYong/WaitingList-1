namespace waitingList.DTOs
{
    public sealed record CentreDto(
        int centreId,
        string externalCentreId,
        string centreName,
        string address,
        decimal latitude,
        decimal longitude,
        int allowedRadiusMetres);
}
