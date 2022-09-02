using Downcast.Authentication.Model;
using Downcast.SessionManager.SDK.Authentication.Extensions;
using Downcast.UserManager.Client.Model;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using AuthenticationRequest = Downcast.Authentication.Model.Input.AuthenticationRequest;

namespace Downcast.Authentication.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationManager _authenticationManager;

    public AuthenticationController(IAuthenticationManager authenticationManager)
    {
        _authenticationManager = authenticationManager;
    }

    /// <summary>
    /// Logs in a user given an email and a password.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public Task<AuthenticationResult> Login(AuthenticationRequest request)
    {
        return _authenticationManager.Login(request);
    }


    /// <summary>
    /// Gets the current logged in user.
    /// </summary>
    /// <returns></returns>
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public Task<User> GetAuthenticatedUser()
    {
        return _authenticationManager.GetUser(User.UserId());
    }
}