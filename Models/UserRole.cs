using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Housemaid.api.Models;

public enum Roles
{
    Tenant,
    Owner,
    Both
}

public class UserRoles
{
    // [ForeignKey(nameof(User))]
    public Guid UserID { get; set; }

    public Roles Role { get; set; }
}