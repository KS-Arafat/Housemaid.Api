using Housemaid.api.Models;

namespace Housemaid.api.Services;

public interface IPropertyAccessService
{
    Task<Housing?> GetOwnedBuildingAsync(Guid houseId, Guid ownerId);
    Task<bool> OwnsBuildingAsync(Guid houseId, Guid ownerId);
    Task<Apartment?> GetOwnedApartmentAsync(Guid apartmentId, Guid ownerId, bool includeTenant = false);
}