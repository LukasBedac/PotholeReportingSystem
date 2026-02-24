using GUIApps.Shared.Services;
using GUIApps.Web.Components;
using GUIApps.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/root/.aspnet/DataProtection-Keys"));

// Add device-specific services used by the GUIApps.Shared project
//builder.Services.AddSingleton<IFormFactor, FormFactor>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(options =>
{
    options.Authority = builder.Configuration["Oidc:Authority"];
    options.ClientId = builder.Configuration["Oidc:ClientId"];
    // If running in Docker, we contact Keycloak via host.docker.internal, but the token issuer is likely localhost.
    var validIssuer = builder.Configuration["Oidc:ValidIssuer"];
    if (!string.IsNullOrEmpty(validIssuer))
    {
        options.TokenValidationParameters.ValidIssuer = validIssuer;
    }

    options.RequireHttpsMetadata = false; // Only for local dev
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    
    // Disable PAR (Pushed Authorization Requests) to make debugging easier
    // and avoid backchannel failures if the redirect_uri doesn't match exactly.
    options.PushedAuthorizationBehavior = PushedAuthorizationBehavior.Disable;

    options.Events = new OpenIdConnectEvents
    {
        OnRedirectToIdentityProvider = context =>
        {
            // Ensure the redirect URI is correct for the environment
            // This is helpful when running in Docker but accessing via localhost
            var request = context.Request;
            var host = request.Host;
            // Console.WriteLine($"[OIDC] Redirect URI being constructed for host: {host}");
            return Task.CompletedTask;
        }
    };

    //options.TokenValidationParameters = new TokenValidationParameters
    //{
    //    NameClaimType = "name",
    //    RoleClaimType = "roles"
    //};

    // Fix for Docker internal networking vs Browser external networking
    // When Keycloak runs with KC_HOSTNAME=localhost, it publishes metadata with "localhost".
    // But inside the Docker container, we must talk to "keycloak" (the service name).
    // This handler intercepts requests to localhost:8080 and rewrites them to keycloak:8080.
    options.BackchannelHttpHandler = new DockerInternalNetworkHandler();
});
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingServerAuthenticationStateProvider>();
builder.Services.AddScoped<GUIApps.Shared.Services.Interfaces.IAuthService, GUIApps.Shared.Services.WebAuthService>();
builder.Services.AddTransient<IClaimsTransformation, KeycloakRolesClaimsTransformation>();

// Register HttpClients for server-side rendering (accessing other containers via Docker network)
builder.Services.AddHttpClient("FileService", client => 
    client.BaseAddress = new Uri("http://filestorage-service:8080"));
builder.Services.AddHttpClient("ReportService", client => 
    client.BaseAddress = new Uri("http://reports-service:8080"));
builder.Services.AddHttpClient("AdminService", client => 
    client.BaseAddress = new Uri("http://admin-service:8080"));

builder.Services.AddControllers();

builder.Services.AddMudServices();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// In Docker local dev, we use HTTP on port 5000. HTTPS redirection can cause issues.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.MapStaticAssets(); // For MudBlazor
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(GUIApps.Shared._Imports).Assembly,
        typeof(GUIApps.Web.Client.Services.PersistentAuthenticationStateProvider).Assembly);

app.Run();

//Helper handler to fix Docker networking issues with Keycloak
public class DockerInternalNetworkHandler : DelegatingHandler
{
    public DockerInternalNetworkHandler()
    {
        InnerHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // If the upstream metadata said "localhost:8080", rewrite it to "keycloak:8080"
        // This allows the container to reach Keycloak using the Docker network name
        if (request.RequestUri?.Host == "localhost" && request.RequestUri?.Port == 8080)
        {
            var builder = new UriBuilder(request.RequestUri)
            {
                Host = "keycloak", // Service name in docker-compose
                Port = 8080
            };
            request.RequestUri = builder.Uri;
            // Console.WriteLine($"[DockerInternalNetworkHandler] Rewrote URL to: {request.RequestUri}");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
