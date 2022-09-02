using Downcast.Authentication.Model;
using Downcast.UserManager.Client.Model;

using AuthenticationRequest = Downcast.Authentication.Model.Input.AuthenticationRequest;

namespace Downcast.Authentication;

public interface IAuthenticationManager
{
    Task<AuthenticationResult> Login(AuthenticationRequest request);
    Task<User> GetUser(string userId);
}