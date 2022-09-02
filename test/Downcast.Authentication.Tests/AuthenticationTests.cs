using System.Net;

using Downcast.Authentication.Client.Model;
using Downcast.Authentication.Tests.Utils;
using Downcast.Authentication.Tests.Utils.DataFakers;
using Downcast.UserManager.Client.Model;

using FluentAssertions;

using Moq;

using Refit;

using AuthenticationRequest = Downcast.Authentication.Client.Model.Input.AuthenticationRequest;

namespace Downcast.Authentication.Tests;

public class AuthenticationTests : BaseTestClass
{
    [Fact]
    public async Task LoginSuccess()
    {
        AuthenticationRequest authRequest = new AuthenticationRequestFaker().Generate();
        SetupCredentialValidationToSucceed(authRequest);

        // Setup user manager to return a specific user when getting user by email
        User user = SetupGetUserByEmailToSucceed(authRequest);

        // act
        ApiResponse<AuthenticationResult> result = await Client.Login(authRequest).ConfigureAwait(false);

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.Content!.User.Should().BeEquivalentTo(user);
        result.Content.TokenInfo.Token.Should().NotBeNullOrEmpty();
        result.Content.TokenInfo.ExpirationDate.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_Invalid_Credentials_Return_Unauthorized()
    {
        AuthenticationRequest authRequest = new AuthenticationRequestFaker().Generate();

        SetupCredentialValidationToFail();

        ApiResponse<AuthenticationResult> result = await Client.Login(authRequest).ConfigureAwait(false);
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task Get_LoggedIn_User_With_Token_Success()
    {
        AuthenticationRequest authRequest = new AuthenticationRequestFaker().Generate();
        SetupCredentialValidationToSucceed(authRequest);

        // Setup user manager to return a specific user when getting user by email
        User user = SetupGetUserByEmailToSucceed(authRequest);
        SetupGetUserByIdToSucceed(user);
        // act
        ApiResponse<AuthenticationResult> loginResult = await Client.Login(authRequest).ConfigureAwait(false);
        loginResult.IsSuccessStatusCode.Should().BeTrue();
        loginResult.Content!.TokenInfo.Token.Should().NotBeNullOrEmpty();

        ApiResponse<User> result =
            await Client.GetLoggedInUser(loginResult.Content!.TokenInfo.Token).ConfigureAwait(false);

        result.IsSuccessStatusCode.Should().BeTrue();
        result.Content!.Should().BeEquivalentTo(user);
    }


    [Fact]
    public async Task Get_LoggedIn_User_With_Token_ThrowsException_On_Invalid_Token()
    {
        ApiResponse<User> result = await Client.GetLoggedInUser("invalid token").ConfigureAwait(false);

        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private void SetupCredentialValidationToFail()
    {
        UserManagerMock
            .Setup(client => client.ValidateCredentials(
                       It.IsAny<UserManager.Client.Model.AuthenticationRequest>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized));
    }


    private User SetupGetUserByEmailToSucceed(AuthenticationRequest authRequest)
    {
        // Setup user manager to return user when getting user by email
        User user = new UserFaker(authRequest.Email).Generate();
        UserManagerMock.Setup(client => client.GetByEmail(user.Email))
            .ReturnsAsync(new ApiResponse<User>(new HttpResponseMessage(HttpStatusCode.OK), user, new RefitSettings()));
        return user;
    }

    private User SetupGetUserByIdToSucceed(User user)
    {
        // Setup user manager to return user when getting user by id
        UserManagerMock.Setup(client => client.GetUser(user.Id))
            .ReturnsAsync(new ApiResponse<User>(new HttpResponseMessage(HttpStatusCode.OK), user, new RefitSettings()));
        return user;
    }

    private void SetupCredentialValidationToSucceed(AuthenticationRequest authRequest)
    {
        // Setup user manager to return OK when validating credentials
        UserManagerMock.Setup(client => client.ValidateCredentials(
                                  It.Is<UserManager.Client.Model.AuthenticationRequest>(
                                      x => x.Email.Equals(authRequest.Email)
                                        && x.Password.Equals(authRequest.Password))))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
    }
}