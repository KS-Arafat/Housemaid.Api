namespace Housemaid.api.Infrastructure;

public static class PropertyCacheTags
{
    public static string OwnerBuildings(Guid ownerId) => $"owner:{ownerId}:buildings";

    public static string OwnerApartments(Guid ownerId) => $"owner:{ownerId}:apartments";

    public static string BuildingListKey(Guid ownerId) => $"buildings:owner:{ownerId}";

    public static string ApartmentListKey(Guid ownerId, Guid houseId) =>
        $"apartments:owner:{ownerId}:building:{houseId}";
}