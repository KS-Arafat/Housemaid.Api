using Housemaid.api.Models;

namespace Housemaid.api.DTO;

public record BuildingListItemDto(
    Guid HouseId,
    string HouseName,
    string Address,
    string City,
    DateOnly CreatedAt);

public record ApartmentListItemDto(
    Guid ApartmentId,
    Guid HouseId,
    Guid? TenantId,
    string UnitNo,
    int Floor,
    int Bedrooms,
    int Bathrooms,
    decimal Area,
    AStatus Status,
    string? Details);