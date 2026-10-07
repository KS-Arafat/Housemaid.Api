using System.ComponentModel.DataAnnotations;

namespace Housemaid.api.DTO;

public class LoginReqDto
{
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}
