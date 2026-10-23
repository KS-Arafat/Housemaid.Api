using Housemaid.api.Models;

namespace Housemaid.api.DTO;

/// <summary>Billing information for one apartment and billing period.</summary>
public sealed record BillingResponseDto(
    Guid BillId,
    Guid ApartmentId,
    decimal Rent,
    decimal? Electricity,
    decimal? Water,
    decimal? Others,
    string? Comments,
    PayStat PaymentStat,
    decimal? AmountPaid,
    DateOnly IssuedAt,
    DateOnly DueAt,
    DateOnly? PaidAt);