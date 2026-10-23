using System.ComponentModel.DataAnnotations;

namespace Housemaid.api.DTO;

public class AssignTenantDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public DateOnly? StartDate { get; set; }

    [Range(typeof(decimal), "0", "100000000000")]
    public decimal Deposit { get; set; }
}