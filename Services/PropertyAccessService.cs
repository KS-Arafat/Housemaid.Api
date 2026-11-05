using Housemaid.api.Data;
using Housemaid.api.Models;
using Microsoft.EntityFrameworkCore;

namespace Housemaid.api.Services;

public class PropertyAccessService(AppDbContext dbContext) : IPropertyAccessService
{
    public Task<Housing?> GetOwnedBuildingAsync(Guid houseId, Guid ownerId) =>
        dbContext.Housings.FirstOrDefaultAsync(housing =>
            housing.HouseId == houseId && housing.OwnerId == ownerId);

    public Task<bool> OwnsBuildingAsync(Guid houseId, Guid ownerId) =>
        dbContext.Housings.AnyAsync(housing =>
            housing.HouseId == houseId && housing.OwnerId == ownerId);

    public Task<Apartment?> GetOwnedApartmentAsync(Guid apartmentId, Guid ownerId, bool includeTenant = false)
    {
        var query = dbContext.Apartments
            .Where(apartment => apartment.ApartmentId == apartmentId && apartment.House.OwnerId == ownerId);

        if (includeTenant)
            query = query.Include(apartment => apartment.Tenant);

        return query.FirstOrDefaultAsync();
    }
}