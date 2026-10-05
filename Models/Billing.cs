using System;

namespace Housemaid.api.Models;

public enum PayStat
{
    Paid,
    Partial,
    Unpaid
}

public class Billing
{
    public Guid BillId { get; set; }
    public Guid TenantId { get; set; }
    public Guid ApartmentId { get; set; }
    public decimal Rent { get; set; }
    public decimal? Electricity { get; set; }
    public decimal? Water { get; set; }
    public decimal? Others { get; set; }
    public string? Comments { get; set; }
    public PayStat PaymentStat { get; set; }
    public decimal? AmountPaid { get; set; }
    public DateOnly IssuedAt { get; set; }
    public DateOnly DueAt { get; set; }
    public DateOnly PaidAt { get; set; }


    public Apartment Apartment { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
}
