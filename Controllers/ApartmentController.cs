using System.Security.Claims;
using Housemaid.api.Constants;
using Housemaid.api.Data;
using Housemaid.api.DTO;
using Housemaid.api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Housemaid.api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class ApartmentController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet("apartment-list/{houseId:Guid}")]
    public async Task<ActionResult> GetAllApartments(Guid houseId)
    {
        var userIdValue = User.FindFirstValue(TokenClaimTypes.NameIdentifier);

        if (userIdValue is null)
            return Unauthorized();

        var userId = Guid.Parse(userIdValue);

        var building = await dbContext.Housings.FirstOrDefaultAsync(h => h.HouseId == houseId && h.OwnerId == userId);

        if (building is null)
            return NotFound("Building Doesn't Exist");

        var apartmentList = await dbContext.Apartments.Where(a => a.HouseId == houseId).ToListAsync();

        return Ok(apartmentList);
    }

    [HttpPost("apartment-add/{houseId:Guid}")]
    public async Task<ActionResult> AddApartments(Guid houseId, [FromBody] List<PostApartmentDto> apartments)
    {
        var userIdValue = User.FindFirstValue(TokenClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized("User is not valid.");

        if (apartments.Count == 0)
            return BadRequest("At least one apartment must be provided.");

        var buildingExists = await dbContext.Housings
            .AnyAsync(h => h.HouseId == houseId && h.OwnerId == userId);

        if (!buildingExists)
            return NotFound("Building not found.");

        var newApartments = apartments.Select(apartment => new Apartment
        {
            HouseId = houseId,
            TenantId = apartment.TenantId,
            UnitNo = apartment.UnitNo,
            Floor = apartment.Floor,
            Bedrooms = apartment.Bedrooms,
            Bathrooms = apartment.Bathrooms,
            Area = apartment.Area,
            Status = apartment.Status,
            Details = apartment.Details
        }).ToList();

        dbContext.Apartments.AddRange(newApartments);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetAllApartments),
            new { houseId },
            newApartments.Select(apartment => new { apartment.ApartmentId, apartment.UnitNo }));
    }

    [HttpPost("assign-tenant/{apartmentId:guid}")]
    public async Task<ActionResult> AssignTenant(Guid apartmentId, [FromBody] AssignTenantDto request)
    {
        var userIdValue = User.FindFirstValue(TokenClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var ownerId))
            return Unauthorized("User is not valid.");

        var apartment = await dbContext.Apartments
            .Include(a => a.House)
            .FirstOrDefaultAsync(a => a.ApartmentId == apartmentId && a.House.OwnerId == ownerId);

        if (apartment is null)
            return NotFound("Apartment not found.");

        if (apartment.TenantId is not null)
            return Conflict("Apartment already has a tenant.");

        if (request.UserId == Guid.Empty)
            return BadRequest("A valid tenant user ID is required.");

        var tenantUserExists = await dbContext.Users.AnyAsync(user => user.Id == request.UserId);
        if (!tenantUserExists)
            return NotFound("Tenant user not found.");

        var alreadyAssigned = await dbContext.Tenants.AnyAsync(tenant => tenant.UserId == request.UserId);
        if (alreadyAssigned)
            return Conflict("User is already assigned as a tenant.");

        var tenant = new Tenant
        {
            UserId = request.UserId,
            HouseId = apartment.HouseId,
            ApartmentId = apartment.ApartmentId,
            StartDate = request.StartDate!.Value,
            Deposit = request.Deposit
        };

        apartment.Tenant = tenant;
        await dbContext.SaveChangesAsync();

        return Ok(new
        {
            tenant.UserId,
            tenant.HouseId,
            tenant.ApartmentId,
            tenant.StartDate,
            tenant.Deposit
        });
    }

    [HttpDelete("remove-tenant/{apartmentId:guid}")]
    public async Task<ActionResult> RemoveTenant(Guid apartmentId)
    {
        var userIdValue = User.FindFirstValue(TokenClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var ownerId))
            return Unauthorized("User is not valid.");

        var apartment = await dbContext.Apartments
            .Include(a => a.House)
            .Include(a => a.Tenant)
            .FirstOrDefaultAsync(a => a.ApartmentId == apartmentId && a.House.OwnerId == ownerId);

        if (apartment is null)
            return NotFound("Apartment not found.");

        if (apartment.Tenant is null)
            return Conflict("Apartment has no assigned tenant.");

        apartment.Tenant.EndDate = DateOnly.FromDateTime(DateTime.UtcNow);
        apartment.TenantId = null;

        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("apartment-remove/{apartmentId:guid}")]
    public async Task<ActionResult> RemoveApartment(Guid apartmentId)
    {
        var userIdValue = User.FindFirstValue(TokenClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var ownerId))
            return Unauthorized("User is not valid.");

        var apartment = await dbContext.Apartments
            .Include(a => a.House)
            .FirstOrDefaultAsync(a => a.ApartmentId == apartmentId && a.House.OwnerId == ownerId);

        if (apartment is null)
            return NotFound("Apartment not found.");

        if (apartment.TenantId is not null)
            return Conflict("Remove the assigned tenant before removing the apartment.");

        var hasBillingHistory = await dbContext.Billings.AnyAsync(billing => billing.ApartmentId == apartmentId);
        if (hasBillingHistory)
            return Conflict("Apartment cannot be removed because it has billing history.");

        dbContext.Apartments.Remove(apartment);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("monthly-billing/{apartmentId:guid}")]
    public async Task<ActionResult<BillingResponseDto>> UpdateMonthlyBilling(
        Guid apartmentId,
        [FromBody] UpdateMonthlyBillingDto request)
    {
        var userIdValue = User.FindFirstValue(TokenClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var ownerId))
            return Unauthorized("User is not valid.");

        var apartment = await dbContext.Apartments
            .Include(a => a.House)
            .FirstOrDefaultAsync(a => a.ApartmentId == apartmentId && a.House.OwnerId == ownerId);

        if (apartment is null)
            return NotFound("Apartment not found.");

        if (apartment.TenantId is not Guid tenantId)
            return Conflict("Apartment has no assigned tenant.");

        var issuedAt = new DateOnly(request.Year, request.Month, 1);
        var nextMonth = issuedAt.AddMonths(1);
        var billing = await dbContext.Billings.FirstOrDefaultAsync(item =>
            item.ApartmentId == apartmentId &&
            item.TenantId == tenantId &&
            item.IssuedAt >= issuedAt &&
            item.IssuedAt < nextMonth);

        var isNew = billing is null;
        billing ??= new Billing
        {
            ApartmentId = apartmentId,
            TenantId = tenantId,
            IssuedAt = issuedAt
        };

        billing.Rent = request.Rent;
        billing.Electricity = request.Electricity;
        billing.Water = request.Water;
        billing.Others = request.Others;
        billing.Comments = request.Comments;
        billing.PaymentStat = request.PaymentStat;
        billing.AmountPaid = request.AmountPaid;
        billing.DueAt = request.DueAt!.Value;
        billing.PaidAt = request.PaidAt ??
            (request.PaymentStat == PayStat.Paid ? DateOnly.FromDateTime(DateTime.UtcNow) : null);

        if (isNew)
            dbContext.Billings.Add(billing);

        await dbContext.SaveChangesAsync();

        return Ok(new BillingResponseDto(
            billing.BillId,
            billing.ApartmentId,
            billing.Rent,
            billing.Electricity,
            billing.Water,
            billing.Others,
            billing.Comments,
            billing.PaymentStat,
            billing.AmountPaid,
            billing.IssuedAt,
            billing.DueAt,
            billing.PaidAt));
    }
}