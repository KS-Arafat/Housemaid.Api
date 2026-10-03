using System.ComponentModel.DataAnnotations.Schema;

namespace Housemaid.api.Models;

public enum AStatus
{
    Listed,
    Unisted
}

public class Apartment
{
    public Guid ApartmeentId { get; set; }

    public Guid HouseId { get; set; }

    public string UnitNo { get; set; } = string.Empty;

    public int Floor { get; set; }

    public int Bedrooms { get; set; }

    public int Bathrooms { get; set; }

    public decimal Area { get; set; }

    public decimal Rent { get; set; }

    public AStatus Status { get; set; }

    public string? Details { get; set; }


}
