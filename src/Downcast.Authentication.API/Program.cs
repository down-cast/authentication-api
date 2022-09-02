using Downcast.Authentication.API.Config;
using Downcast.Common.Errors.Handler.Config;
using Downcast.Common.Logging;
using Downcast.SessionManager.SDK.Authentication.Handler;

using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("http-clients-settings.json");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerConfig();

builder.Services.AddDowncastAuthentication(builder.Configuration);

builder.AddAuthenticationServices();
builder.AddSerilog();
builder.AddErrorHandlerOptions();

WebApplication app = builder.Build();

app.UseSerilogRequestLogging();
app.UseErrorHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseForwardedHeaders();

app.MapControllers();

app.Run();