using Downcast.SessionManager.SDK.Client.Model;
using Downcast.UserManager.Client.Model;

namespace Downcast.Authentication.Model;

public class AuthenticationResult
{
    public TokenResult TokenInfo { get; init; }
    public User User { get; init; }

    public AuthenticationResult(TokenResult tokenInfo, User user)
    {
        TokenInfo = tokenInfo;
        User = user;
    }
}