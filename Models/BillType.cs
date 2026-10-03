namespace Housemaid.api.Models;


public class BillType
{
    public required string BillName { get; set; } = string.Empty;
    public required decimal Amount { get; set; }
}