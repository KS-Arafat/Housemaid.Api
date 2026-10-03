using System;
using System.ComponentModel.DataAnnotations;

namespace Housemaid.api.Models;

public class User
{
    public Guid UserId { get; set; }

    public required string FullName { get; set; } = string.Empty;

    public required string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public DateOnly CreatedAt { get; set; }

}
