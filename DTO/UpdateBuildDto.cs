using System.ComponentModel.DataAnnotations;

namespace Housemaid.api.DTO;

public class UpdateBuildDto
{
    [StringLength(100, MinimumLength = 1)]
    public string? HouseName { get; set; }

    [StringLength(50, MinimumLength = 1)]
    public string? Address { get; set; }

    [StringLength(15, MinimumLength = 1)]
    public string? City { get; set; }
}