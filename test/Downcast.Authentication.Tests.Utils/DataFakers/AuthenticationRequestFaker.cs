using Bogus;

using Downcast.Authentication.Client.Model.Input;

namespace Downcast.Authentication.Tests.Utils.DataFakers;

public sealed class AuthenticationRequestFaker : Faker<AuthenticationRequest>
{
    public AuthenticationRequestFaker()
    {
        RuleFor(auth => auth.Email, faker => faker.Internet.Email());
        RuleFor(auth => auth.Password, faker => faker.Internet.Password());
    }
}