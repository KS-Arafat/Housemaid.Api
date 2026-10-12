using System.ComponentModel.DataAnnotations;

namespace Housemaid.api.DTO;

public class UpdateBuildDto
{
    [Required]
    public Guid? HouseId { get; set; }

    [StringLength(100, MinimumLength = 1)]
    public string? HouseName { get; set; }

    [StringLength(50, MinimumLength = 1)]
    public string? Address { get; set; }

    [StringLength(15, MinimumLength = 1)]
    public string? City { get; set; }
}

public class DeleteBuildDto
{
    [Required]
    public required Guid? HouseId { get; set; }
}