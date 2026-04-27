using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using GUIApps.Web.Services;
using Xunit;

namespace GUIApps.Tests
{
    public class KeycloakRolesClaimsTransformationTests
    {
        private readonly KeycloakRolesClaimsTransformation _transformation;

        public KeycloakRolesClaimsTransformationTests()
        {
            _transformation = new KeycloakRolesClaimsTransformation();
        }

        [Fact]
        public async Task TransformAsync_With_RealmAccessRoles_Should_Add_RoleClaims()
        {
            // Arrange
            var identity = new ClaimsIdentity("TestAuthType");
            var realmAccessJson = "{\"roles\": [\"admin\", \"user\"]}";
            identity.AddClaim(new Claim("realm_access", realmAccessJson));
            var principal = new ClaimsPrincipal(identity);

            // Act
            var resultPrincipal = await _transformation.TransformAsync(principal);

            // Assert
            resultPrincipal.HasClaim(ClaimTypes.Role, "admin").Should().BeTrue();
            resultPrincipal.HasClaim(ClaimTypes.Role, "user").Should().BeTrue();
        }

        [Fact]
        public async Task TransformAsync_With_NoRoles_Should_Not_Throw_Or_Add_Roles()
        {
            // Arrange
            var identity = new ClaimsIdentity("TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            // Act
            var resultPrincipal = await _transformation.TransformAsync(principal);

            // Assert
            resultPrincipal.Claims.Should().NotContain(c => c.Type == ClaimTypes.Role);
        }

        [Fact]
        public async Task TransformAsync_With_InvalidJson_Should_Handle_Gracefully()
        {
            // Arrange
            var identity = new ClaimsIdentity("TestAuthType");
            identity.AddClaim(new Claim("realm_access", "invalid-json"));
            var principal = new ClaimsPrincipal(identity);

            // Act
            var resultPrincipal = await _transformation.TransformAsync(principal);

            // Assert
            resultPrincipal.Claims.Should().NotContain(c => c.Type == ClaimTypes.Role);
        }
    }
}
