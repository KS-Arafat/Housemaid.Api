using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Housemaid.api.Models;

public class Housing
{
    public Guid HouseId { get; set; }
    public Guid OwnerId { get; set; }
    public Guid TenantId { get; set; }
    public required string HouseName { get; set; } = string.Empty;
    public required string Address { get; set; } = string.Empty;
    public required string City { get; set; } = string.Empty;
    public DateOnly CreatedAt { get; set; }

    public User Owner { get; set; } = null!;
    public List<Apartment> Apartments { get; set; } = [];
    public List<Tenant> Tenants { get; set; } = [];
}
