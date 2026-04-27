using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
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
using Moq.Protected;
using MudBlazor;
using MudBlazor.Services;
using Xunit;

namespace GUIApps.Tests
{
    public class MapViewTests : TestContext, IAsyncLifetime
    {
        public MapViewTests()
        {
            Services.AddMudServices();

            var mockLocalizer = new Mock<IStringLocalizer<AppLanguage>>();
            mockLocalizer.Setup(x => x[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));
            Services.AddSingleton(mockLocalizer.Object);

            var mockSnackbar = new Mock<ISnackbar>();
            Services.AddSingleton(mockSnackbar.Object);

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
        public async Task MapView_Should_Call_InitMap_JSInterop_With_Correct_Data()
        {
            // Arrange
            var reports = new List<Report>
            {
                new Report
                {
                    UserID = "test-user",
                    Latitude = 48.5,
                    LongTitude = 17.5,
                    Category = PotholeCategory.Pothole,
                    Status = ReportStatus.Reported,
                    Description = "Test Pothole",
                    Image = new FileMinio { BucketName = "", StorageKey = "" }
                }
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<System.Threading.CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(reports))
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new System.Uri("http://localhost/")
            };

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(_ => _.CreateClient("ReportService")).Returns(httpClient);
            Services.AddSingleton(mockHttpClientFactory.Object);

            // Act
            var cut = RenderComponent<MapView>();
            
            // Allow OnAfterRenderAsync to complete
            await Task.Delay(100); // Give small time for async operation to complete since it involves network mock

            // Assert
            var invocation = JSInterop.VerifyInvoke("initMap");
            invocation.Arguments[0].Should().Be("map");

            // Second argument should be our markers array
            var markersArg = invocation.Arguments[1];
            markersArg.Should().NotBeNull();

            // There shouldn't be any Loading spinner present anymore
            cut.FindAll("div.mud-progress-circular").Should().BeEmpty();
        }
    }
}
