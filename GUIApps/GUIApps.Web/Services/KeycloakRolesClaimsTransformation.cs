using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace GUIApps.Web.Services;

public class KeycloakRolesClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return Task.FromResult(principal);
        }

        var identity = (ClaimsIdentity)principal.Identity;

        var rolesClaim = principal.FindFirst("realm_access");
        if (rolesClaim != null && !string.IsNullOrWhiteSpace(rolesClaim.Value))
        {
            try
            {
                using var jsonDoc = JsonDocument.Parse(rolesClaim.Value);
                if (jsonDoc.RootElement.TryGetProperty("roles", out var roles) && roles.ValueKind == JsonValueKind.Array)
                {
                    foreach (var role in roles.EnumerateArray())
                    {
                        if (role.ValueKind == JsonValueKind.String)
                        {
                            var roleValue = role.GetString();
                            if (!string.IsNullOrEmpty(roleValue))
                            {
                                if (!principal.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == roleValue))
                                {
                                    identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Ignore parsing errors
            }
        }

        return Task.FromResult(principal);
    }
}
