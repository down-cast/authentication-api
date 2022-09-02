using Downcast.Authentication.Client.Model;
using Downcast.UserManager.Client.Model;

using Refit;

using AuthenticationRequest = Downcast.Authentication.Client.Model.Input.AuthenticationRequest;

namespace Downcast.Authentication.Client;

public interface IAuthenticationClient
{
    /// <summary>
    /// Logs in a user given an email and a password.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Post("/api/v1/authentication/login")]
    Task<ApiResponse<AuthenticationResult>> Login([Body] AuthenticationRequest request);

    /// <summary>
    /// Gets the current logged in user.
    /// </summary>
    /// <returns></returns>
    [Get("/api/v1/authentication/me")]
    Task<ApiResponse<User>> GetLoggedInUser([Authorize] string token);
}