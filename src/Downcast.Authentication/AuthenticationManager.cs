using Downcast.Authentication.Model;
using Downcast.Common.Errors;
using Downcast.SessionManager.SDK.Authentication.Extensions;
using Downcast.SessionManager.SDK.Client;
using Downcast.SessionManager.SDK.Client.Model;
using Downcast.UserManager.Client;
using Downcast.UserManager.Client.Model;

using Microsoft.Extensions.Logging;

using Refit;

using AuthenticationRequest = Downcast.Authentication.Model.Input.AuthenticationRequest;

namespace Downcast.Authentication;

public class AuthenticationManager : IAuthenticationManager
{
    private readonly ISessionManagerClient _sessionManagerClient;
    private readonly IUserManagerClient _userManagerClient;
    private readonly ILogger<AuthenticationManager> _logger;

    public AuthenticationManager(
        ISessionManagerClient sessionManagerClient,
        IUserManagerClient userManagerClient,
        ILogger<AuthenticationManager> logger)
    {
        _sessionManagerClient = sessionManagerClient;
        _userManagerClient = userManagerClient;
        _logger = logger;
    }


    public async Task<User> GetUser(string userId)
    {
        ApiResponse<User> userResponse = await _userManagerClient.GetUser(userId).ConfigureAwait(false);

        userResponse = await userResponse.EnsureSuccessStatusCodeAsync().ConfigureAwait(false);
        return userResponse.Content!;
    }

    public async Task<AuthenticationResult> Login(AuthenticationRequest request)
    {
        bool credentialsValid = await CredentialsAreValid(request).ConfigureAwait(false);
        if (credentialsValid is false)
        {
            _logger.LogInformation("User with email {Email} failed to login", request.Email);
            throw new DcException(ErrorCodes.AuthenticationFailed, "Invalid credentials");
        }

        User user = await GetUserByEmail(request.Email).ConfigureAwait(false);
        TokenResult session = await CreateSessionForUser(user).ConfigureAwait(false);
        return new AuthenticationResult(session, user);
    }

    private Task<TokenResult> CreateSessionForUser(User user)
    {
        return _sessionManagerClient.CreateSessionToken(new Dictionary<string, object>()
        {
            { ClaimNames.UserId, user.Id },
            { ClaimNames.Email, user.Email },
            { ClaimNames.DisplayName, user.DisplayName ?? "" },
            { ClaimNames.ProfilePictureUri, user.ProfilePictureUri?.ToString() ?? "" },
            { ClaimNames.Role, user.Roles }
        });
    }

    private async Task<User> GetUserByEmail(string email)
    {
        ApiResponse<User> userResponse = await _userManagerClient.GetByEmail(email).ConfigureAwait(false);
        if (userResponse.IsSuccessStatusCode)
        {
            return userResponse.Content;
        }

        _logger.LogError("Could not get user with email {Email}", email);
        throw new DcException(ErrorCodes.EntityNotFound, "Could not find user by email");
    }


    private async Task<bool> CredentialsAreValid(AuthenticationRequest request)
    {
        HttpResponseMessage response = await _userManagerClient
            .ValidateCredentials(new UserManager.Client.Model.AuthenticationRequest
            {
                Password = request.Password,
                Email = request.Email
            }).ConfigureAwait(false);

        return response.IsSuccessStatusCode;
    }
}