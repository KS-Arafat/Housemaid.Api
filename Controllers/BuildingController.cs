using System.Security.Claims;
using Housemaid.api.Constants;
using Housemaid.api.Data;
using Housemaid.api.DTO;
using Housemaid.api.Models;
using Housemaid.api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Housemaid.api.Controllers;


[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class BuildingController(AppDbContext dbContext, IPropertyAccessService propertyAccess) : ControllerBase
{
    [HttpGet("my-builds")]
    public async Task<ActionResult> GetBuildingList()
    {
        var userIdValue = User.FindFirstValue(TokenClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized("User is not valid.");

        var buildings = await dbContext.Housings
            .Where(housing => housing.OwnerId == userId)
            .Select(housing => new
            {
                housing.HouseId,
                housing.HouseName,
                housing.Address,
                housing.City,
                housing.CreatedAt
            })
            .ToListAsync();

        return Ok(buildings);
    }

    [HttpPost("add-build")]
    public async Task<ActionResult> AddBuildingList([FromBody] PostBuildingDto req)
    {

        var userIdValue = User.FindFirstValue(TokenClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized("User Not Valid");

        var newBuilding = new Housing
        {
            OwnerId = userId,
            HouseName = req.HouseName,
            Address = req.Address,
            City = req.City,
            CreatedAt = DateOnly.FromDateTime(DateTime.Today)
        };

        dbContext.Housings.Add(newBuilding);
        try
        {
            await dbContext.SaveChangesAsync();
            return Ok(new
            {
                newBuilding.HouseId
            });
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex);
            return Problem();
        }
    }

    [HttpPut("update-build/{houseId:guid}")]
    public async Task<ActionResult> UpdateBuildDetails(Guid houseId, [FromBody] UpdateBuildDto req)
    {
        var userIdValue = User.FindFirstValue(TokenClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized("User is not valid.");

        if (req.HouseName is null && req.Address is null && req.City is null)
            return BadRequest("At least one building field must be provided.");

        var building = await propertyAccess.GetOwnedBuildingAsync(houseId, userId);

        if (building is null)
            return NotFound("Building not found.");

        building.HouseName = req.HouseName ?? building.HouseName;
        building.Address = req.Address ?? building.Address;
        building.City = req.City ?? building.City;

        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("delete-build/{houseId:guid}")]
    public async Task<ActionResult> DeleteBuild(Guid houseId)
    {

        var userIdValue = User.FindFirstValue(TokenClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized("User is not valid.");

        var building = await propertyAccess.GetOwnedBuildingAsync(houseId, userId);
        if (building is null)
            return NotFound("Building Not Found");

        dbContext.Housings.Remove(building);
        await dbContext.SaveChangesAsync();


        return Ok();
    }

}
