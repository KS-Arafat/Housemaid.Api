using System.ComponentModel.DataAnnotations.Schema;

namespace Housemaid.api.Models;

public enum AStatus
{
    Listed,
    Unlisted
}

public class Apartment
{
    public Guid ApartmentId { get; set; }
    public Guid HouseId { get; set; }
    public Guid TenantId { get; set; }

    public string UnitNo { get; set; } = string.Empty;
    public int Floor { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public decimal Area { get; set; }
    public AStatus Status { get; set; }
    public string? Details { get; set; }

    public Housing House { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
    public List<Billing> Billings { get; set; } = [];
}
