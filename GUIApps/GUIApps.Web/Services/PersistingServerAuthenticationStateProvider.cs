using GUIApps.Shared.Models; // Ensure this matches your namespace for UserInfo
using GUIApps.Models.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace GUIApps.Web.Services;

public class PersistingServerAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly PersistentComponentState _state;
    private readonly PersistingComponentStateSubscription _subscription;

    public PersistingServerAuthenticationStateProvider(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory scopeFactory,
        PersistentComponentState state)
        : base(loggerFactory)
    {
        _scopeFactory = scopeFactory;
        _state = state;
        // Register the persistence callback
        _subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    // This checks if the user is still valid in the background (e.g. check DB or Cookie validity)
    protected override Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        // For Keycloak/Cookie auth, we assume valid as long as the cookie exists. 
        // We can add logic here to check if the session is still active if needed.
        return Task.FromResult(true);
    }

    private async Task OnPersistingAsync()
    {
        var authenticationState = await GetAuthenticationStateAsync();
        var principal = authenticationState.User;
        var rolesClaim = principal.FindFirst("realm_access");

        if (principal.Identity?.IsAuthenticated == true)
        {
            var userId = principal.FindFirst("sub")?.Value
                         ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = principal.FindFirst("email")?.Value
                        ?? principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = principal.FindFirst("name")?.Value
                       ?? principal.FindFirst(ClaimTypes.Name)?.Value;

            if (userId != null)
            {
                var userInfo = new UserInfo
                {
                    UserId = userId, 
                    Name = name ?? "",
                    Email = email ?? "",
                };

                if (rolesClaim != null)
                {
                    try
                    {
                        using (var jsonDoc = JsonDocument.Parse(rolesClaim.Value))
                        {
                            if (jsonDoc.RootElement.TryGetProperty("roles", out var roles) && roles.ValueKind == JsonValueKind.Array)
                            {
                                userInfo.Roles = roles.EnumerateArray()
                                                    .Where(r => r.ValueKind == JsonValueKind.String)
                                                    .Select(r => r.GetString()!)
                                                    .Where(r => Enum.TryParse<UserRole>(r, true, out _))
                                                    .Select(r => Enum.Parse<UserRole>(r, true))
                                                    .ToList();
                            }
                        }
                    }
                    catch
                    {
                        //Ignore JSON parse errors }
                    }
                }
                // Persist the data so the WASM client can pick it up
                _state.PersistAsJson("UserInfo", userInfo);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        _subscription.Dispose();
        base.Dispose(disposing);
    }
}