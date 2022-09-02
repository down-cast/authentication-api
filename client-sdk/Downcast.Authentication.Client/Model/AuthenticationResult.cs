using Downcast.SessionManager.SDK.Client.Model;
using Downcast.UserManager.Client.Model;

namespace Downcast.Authentication.Client.Model;

public class AuthenticationResult
{
    public TokenResult TokenInfo { get; init; } = null!;
    public User User { get; init; } = null!;
}