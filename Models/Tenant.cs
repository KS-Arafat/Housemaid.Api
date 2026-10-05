using System;

namespace Housemaid.api.Models;

public class Tenant
{
    public Guid? UserId { get; set; }
    public Guid HouseId { get; set; }
    public Guid ApartmentId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal Deposit { get; set; }

    public Housing House { get; set; } = null!;
    public Apartment Apartment { get; set; } = null!;
    public User User { get; set; } = null!;
    public List<Billing> Billings { get; set; } = [];
}
