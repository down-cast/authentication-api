using Bogus;

using Downcast.Authentication.Client;
using Downcast.SessionManager.SDK.Client;
using Downcast.UserManager.Client;

using Moq;

using Refit;

namespace Downcast.Authentication.Tests.Utils;

public class BaseTestClass
{
    protected Faker Faker { get; } = new();
    protected Mock<ISessionManagerClient> SessionManagerMock { get; } = new(MockBehavior.Strict);
    protected Mock<IUserManagerClient> UserManagerMock { get; } = new(MockBehavior.Strict);
    protected IAuthenticationClient Client { get; }

    public BaseTestClass()
    {
        HttpClient httpClient = new AuthenticationServerInstance(UserManagerMock, SessionManagerMock).CreateClient();
        Client = RestService.For<IAuthenticationClient>(httpClient);
    }
}