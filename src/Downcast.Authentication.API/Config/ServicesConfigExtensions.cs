using Downcast.UserManager.Client.DependencyInjection;

namespace Downcast.Authentication.API.Config;

public static class ServicesConfigurationExtensions
{
    public static WebApplicationBuilder AddAuthenticationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAuthenticationManager, AuthenticationManager>();
        builder.Services.AddUserManagerClient(builder.Configuration);
        return builder;
    }
}