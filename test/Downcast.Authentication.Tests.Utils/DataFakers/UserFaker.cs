using Bogus;

using Downcast.UserManager.Client.Model;

namespace Downcast.Authentication.Tests.Utils.DataFakers;

public sealed class UserFaker : Faker<User>
{
    public UserFaker(string email)
    {
        RuleForType(typeof(Uri), faker => new Uri(faker.Internet.Avatar()));
        RuleFor(u => u.Id, faker => faker.Random.Guid().ToString());
        RuleFor(u => u.DisplayName, faker => faker.Name.FullName());
        RuleFor(u => u.Email, email);
        RuleFor(u => u.Description, faker => faker.Lorem.Paragraph());
        RuleFor(u => u.Created, faker => faker.Date.Recent());
        RuleFor(u => u.Updated, faker => faker.Date.Recent());
        RuleFor(u => u.Roles, faker => new[] { "member", "admin" });
    }
}