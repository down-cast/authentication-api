using System.ComponentModel.DataAnnotations;

namespace Downcast.Authentication.Model.Input;

public class AuthenticationRequest
{
    [Required(AllowEmptyStrings = false)]
    public string Password { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    [EmailAddress]
    public string Email { get; init; } = null!;
}