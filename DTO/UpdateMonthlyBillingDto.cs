using System.ComponentModel.DataAnnotations;
using Housemaid.api.Models;

namespace Housemaid.api.DTO;

/// <summary>Monthly charges and payment status set by the apartment owner.</summary>
public sealed record UpdateMonthlyBillingDto
{
    [Range(1, 9999)]
    public int Year { get; init; }

    [Range(1, 12)]
    public int Month { get; init; }

    [Range(typeof(decimal), "0", "100000000000")]
    public decimal Rent { get; init; }

    [Range(typeof(decimal), "0", "100000000000")]
    public decimal? Electricity { get; init; }

    [Range(typeof(decimal), "0", "100000000000")]
    public decimal? Water { get; init; }

    [Range(typeof(decimal), "0", "100000000000")]
    public decimal? Others { get; init; }

    [StringLength(1000)]
    public string? Comments { get; init; }

    [Required]
    public DateOnly? DueAt { get; init; }

    [EnumDataType(typeof(PayStat))]
    public PayStat PaymentStat { get; init; } = PayStat.Unpaid;

    [Range(typeof(decimal), "0", "100000000000")]
    public decimal? AmountPaid { get; init; }

    public DateOnly? PaidAt { get; init; }
}