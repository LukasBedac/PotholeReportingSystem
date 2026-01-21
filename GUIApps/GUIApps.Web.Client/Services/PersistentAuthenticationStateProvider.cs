using System.Security.Claims;
using GUIApps.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace GUIApps.Web.Client.Services;

public class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> _defaultUnauthenticatedTask =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private readonly Task<AuthenticationState> _authenticationStateTask = _defaultUnauthenticatedTask;

    public PersistentAuthenticationStateProvider(PersistentComponentState state)
    {
        if (!state.TryTakeFromJson<UserInfo>("UserInfo", out var userInfo) || userInfo is null)
        {
            return;
        }

        // Reconstruct the ClaimsPrincipal from the UserInfo DTO
        List<Claim> claims = [
            new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
            new Claim(ClaimTypes.Name, userInfo.Name),
            new Claim(ClaimTypes.Email, userInfo.Email)
        ];


        foreach (var role in userInfo.Roles)
        {
            claims.Add(new Claim("roles", role));
        }
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "PersistentAuth", "name", "role"));
        _authenticationStateTask = Task.FromResult(new AuthenticationState(authenticatedUser));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => _authenticationStateTask;
}