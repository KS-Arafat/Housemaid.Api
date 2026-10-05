using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Housemaid.api.Models;

public class User : IdentityUser<Guid>
{
    public DateOnly CreatedAt { get; set; }

    public Tenant Tenant { get; set; } = null!;
    public List<Housing> Housings { get; set; } = [];
    public List<Apartment> Apartments { get; set; } = [];
}
