using System;

namespace Housemaid.api.Models;

public class Tenant
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid ApartmentId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public decimal Deposit { get; set; }

}
