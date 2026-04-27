using Bunit;
using FluentAssertions;
using GUIApps.Shared.Components;
using GUIApps.Shared.Resources;
using GUIApps.Shared.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Moq;
using MudBlazor;
using MudBlazor.Services;
using Xunit;

namespace GUIApps.Tests
{
    public class NotAuthorizedTests : TestContext, IAsyncLifetime
    {
        public NotAuthorizedTests()
        {
            Services.AddMudServices();

            var mockLocalizer = new Mock<IStringLocalizer<AppLanguage>>();
            mockLocalizer.Setup(x => x[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));
            Services.AddSingleton(mockLocalizer.Object);

            JSInterop.Mode = JSRuntimeMode.Loose;
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
        public void NotAuthorized_Component_Should_Render_AccessDenied_Message()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();
            Services.AddSingleton(authServiceMock.Object);

            // Act
            var cut = RenderComponent<NotAuthorized>();

            // Assert
            cut.Markup.Should().Contain("Access Denied");
            cut.Markup.Should().Contain("You must log in to access this section.");

            var loginButton = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Login"));
            loginButton.Should().NotBeNull();
        }

        [Fact]
        public void NotAuthorized_Component_LoginButton_Should_Invoke_AuthService()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();
            authServiceMock.Setup(s => s.LoginAsync()).Returns(Task.CompletedTask);
            Services.AddSingleton(authServiceMock.Object);

            var cut = RenderComponent<NotAuthorized>();
            var loginButton = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Login"));

            // Act
            loginButton?.Click();

            // Assert
            authServiceMock.Verify(s => s.LoginAsync(), Times.Once);
        }
    }
}
