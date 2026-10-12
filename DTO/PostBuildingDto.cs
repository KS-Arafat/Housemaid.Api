using System.ComponentModel.DataAnnotations;

namespace Housemaid.api.DTO;

public class PostBuildingDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string HouseName { get; set; } = string.Empty;
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public required string Address { get; set; } = string.Empty;
    [Required]
    [StringLength(15, MinimumLength = 1)]
    public required string City { get; set; } = string.Empty;
}
