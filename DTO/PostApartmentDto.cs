using System.ComponentModel.DataAnnotations;
using Housemaid.api.Models;

namespace Housemaid.api.DTO;

public class PostApartmentDto
{
    [Required]
    [StringLength(30, MinimumLength = 1)]
    public required string UnitNo { get; set; }

    public Guid? TenantId { get; set; }

    public int Floor { get; set; }

    [Range(0, int.MaxValue)]
    public int Bedrooms { get; set; }

    [Range(0, int.MaxValue)]
    public int Bathrooms { get; set; }

    [Range(typeof(decimal), "0.01", "100000000000")]
    public decimal Area { get; set; }

    [EnumDataType(typeof(AStatus))]
    public AStatus Status { get; set; }

    [StringLength(200)]
    public string? Details { get; set; }
}