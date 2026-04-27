using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using GUIApps.Shared.Models;
using GUIApps.Shared.Models.Enums;
using GUIApps.Shared.Pages;
using GUIApps.Shared.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Moq;
using MudBlazor;
using MudBlazor.Services;
using Xunit;

namespace GUIApps.Tests
{
    public class CreateReportTests : TestContext, IAsyncLifetime
    {
        public CreateReportTests()
        {
            Services.AddMudServices();

            var mockLocalizer = new Mock<IStringLocalizer<AppLanguage>>();
            mockLocalizer.Setup(x => x[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));
            Services.AddSingleton(mockLocalizer.Object);

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            Services.AddSingleton(mockHttpClientFactory.Object);

            var mockSnackbar = new Mock<ISnackbar>();
            Services.AddSingleton(mockSnackbar.Object);

            var popoverProvider = new Mock<IPopoverObserver>();
            Services.AddSingleton(popoverProvider.Object);
            JSInterop.Mode = JSRuntimeMode.Loose;

            // Mock Authorization state
            var claims = new[] { new Claim(ClaimTypes.Name, "test-user") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var mockAuthStateProvider = new Mock<AuthenticationStateProvider>();
            mockAuthStateProvider.Setup(p => p.GetAuthenticationStateAsync())
                .ReturnsAsync(new AuthenticationState(claimsPrincipal));
            Services.AddSingleton(mockAuthStateProvider.Object);
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            if (Services is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
        }

        [Fact]
        public void CreateReport_SubmitButton_Should_BeDisabled_Initially()
        {
            // Arrange & Act
            ComponentFactories.AddStub<MudFileUpload<IBrowserFile>>();
            ComponentFactories.AddStub<MudSelect<string>>();
            ComponentFactories.AddStub<MudSelect<PotholeCategory>>();
            ComponentFactories.AddStub<MudSelect<ReportStatus>>();
            ComponentFactories.AddStub<MudTextField<string>>();

            var cut = RenderComponent<CreateReport>();

            // Assert
            var submitButton = cut.Find("button[type='submit']");
            submitButton.HasAttribute("disabled").Should().BeTrue("The submit button should be disabled when mandatory fields are missing.");
        }
    }
}
