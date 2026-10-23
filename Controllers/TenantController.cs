using System.Security.Claims;
using Housemaid.api.Constants;
using Housemaid.api.Data;
using Housemaid.api.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Housemaid.api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class TenantController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet("billing/current-month")]
    public async Task<ActionResult<List<BillingResponseDto>>> GetCurrentMonthBilling()
    {
        if (!Guid.TryParse(User.FindFirstValue(TokenClaimTypes.NameIdentifier), out var tenantUserId))
            return Unauthorized("User is not valid.");

        var tenantExists = await dbContext.Tenants.AnyAsync(tenant => tenant.UserId == tenantUserId);
        if (!tenantExists)
            return NotFound("Tenant record not found.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var monthStart = new DateOnly(today.Year, today.Month, 1);
        var nextMonth = monthStart.AddMonths(1);

        var billing = await dbContext.Billings
            .Where(item => item.TenantId == tenantUserId && item.IssuedAt >= monthStart && item.IssuedAt < nextMonth)
            .OrderBy(item => item.ApartmentId)
            .Select(item => new BillingResponseDto(
                item.BillId,
                item.ApartmentId,
                item.Rent,
                item.Electricity,
                item.Water,
                item.Others,
                item.Comments,
                item.PaymentStat,
                item.AmountPaid,
                item.IssuedAt,
                item.DueAt,
                item.PaidAt))
            .ToListAsync();

        return Ok(billing);
    }

    [HttpGet("billing")]
    public async Task<ActionResult<List<BillingResponseDto>>> GetBillingHistory(
        [FromQuery] int? year,
        [FromQuery] int? month)
    {
        if (!Guid.TryParse(User.FindFirstValue(TokenClaimTypes.NameIdentifier), out var tenantUserId))
            return Unauthorized("User is not valid.");

        if (year is < 1 or > 9999 || month is < 1 or > 12)
            return BadRequest("Year must be between 1 and 9999 and month must be between 1 and 12.");

        var tenantExists = await dbContext.Tenants.AnyAsync(tenant => tenant.UserId == tenantUserId);
        if (!tenantExists)
            return NotFound("Tenant record not found.");

        var query = dbContext.Billings.Where(item => item.TenantId == tenantUserId);

        if (year.HasValue && month.HasValue)
        {
            var monthStart = new DateOnly(year.Value, month.Value, 1);
            var nextMonth = monthStart.AddMonths(1);
            query = query.Where(item => item.IssuedAt >= monthStart && item.IssuedAt < nextMonth);
        }
        else if (year.HasValue)
        {
            var yearStart = new DateOnly(year.Value, 1, 1);
            var nextYear = yearStart.AddYears(1);
            query = query.Where(item => item.IssuedAt >= yearStart && item.IssuedAt < nextYear);
        }
        else if (month.HasValue)
        {
            query = query.Where(item => item.IssuedAt.Month == month.Value);
        }

        var billing = await query
            .OrderByDescending(item => item.IssuedAt)
            .Select(item => new BillingResponseDto(
                item.BillId,
                item.ApartmentId,
                item.Rent,
                item.Electricity,
                item.Water,
                item.Others,
                item.Comments,
                item.PaymentStat,
                item.AmountPaid,
                item.IssuedAt,
                item.DueAt,
                item.PaidAt))
            .ToListAsync();

        return Ok(billing);
    }

}

