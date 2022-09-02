using System.Net;
using System.Text;

using Downcast.Authentication.API.Controllers;
using Downcast.SessionManager.SDK.Client;
using Downcast.SessionManager.SDK.Client.Model;
using Downcast.UserManager.Client;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using Moq;

using Refit;

namespace Downcast.Authentication.Tests.Utils;

public class AuthenticationServerInstance : WebApplicationFactory<AuthenticationController>
{
    private readonly JsonWebTokenHandler _handler = new();
    private readonly Mock<ISessionManagerClient> _sessionManagerMock;
    private readonly Mock<IUserManagerClient> _userManagerClient;

    public AuthenticationServerInstance(
        Mock<IUserManagerClient> userManagerClient,
        Mock<ISessionManagerClient> sessionManagerMock)
    {
        _userManagerClient = userManagerClient;
        _sessionManagerMock = sessionManagerMock;
        SetupTokenValidation();
        SetupTokenCreation();
    }

    private void SetupTokenValidation()
    {
        _sessionManagerMock.Setup(client => client.ValidateSessionToken(It.IsAny<string>()))
            .Returns<string>(token =>
            {
                try
                {
                    _handler.ReadJsonWebToken(token);
                    return Task.CompletedTask;
                }
                catch (Exception)
                {
                    throw ApiException.Create(
                            new HttpRequestMessage(HttpMethod.Post, "http://localhost"),
                            HttpMethod.Post,
                            new HttpResponseMessage(HttpStatusCode.Unauthorized), new RefitSettings())
                        .Result;
                }
            });
    }

    private void SetupTokenCreation()
    {
        _sessionManagerMock.Setup(client => client.CreateSessionToken(It.IsAny<IDictionary<string, object>>()))
            .Returns<IDictionary<string, object>>(claims =>
            {
                DateTime expirationDate = DateTime.UtcNow.AddDays(4);
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("dummy keyyyyyyyyyyyyyyyy"));
                var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                string? token = _handler.CreateToken(new SecurityTokenDescriptor
                {
                    Expires = expirationDate,
                    Claims = claims,
                    SigningCredentials = signingCredentials,
                    IssuedAt = DateTime.UtcNow,
                });

                return Task.FromResult(new TokenResult
                {
                    Token = token,
                    ExpirationDate = expirationDate
                });
            });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton(_sessionManagerMock.Object);
            services.AddSingleton(_userManagerClient.Object);
        });
    }
}