using System;

namespace Housemaid.api.Models;

public class Billing
{
    public Guid BillId { get; set; }
    public Guid UserId { get; set; }
    public Guid ApratId { get; set; }

    public List<BillType>? BillList { get; set; }
}
