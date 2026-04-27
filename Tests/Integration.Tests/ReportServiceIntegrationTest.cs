using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using GUIApps.Shared.Models;
using GUIApps.Shared.Models.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportVoteService.Database;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;
using Xunit;

namespace Integration.Tests
{
    public class ReportServiceIntegrationTest : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer;
        private readonly KafkaContainer _kafkaContainer;

        public ReportServiceIntegrationTest()
        {
            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres:15-alpine")
                .WithDatabase("testdb")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();

            _kafkaContainer = new KafkaBuilder()
                .WithImage("confluentinc/cp-kafka:7.4.0")
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            await _kafkaContainer.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _dbContainer.StopAsync();
            await _kafkaContainer.StopAsync();
        }

        [Fact]
        public async Task CreateReport_ReturnsOk_AndSavesToDb()
        {
            // Arrange
            var connectionString = _dbContainer.GetConnectionString();
            var kafkaBootstrapServers = _kafkaContainer.GetBootstrapAddress();

            // WebApplicationFactory handles spinning up the test server
            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Remove the existing db context registration
                        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ReportsVoteDbContext>));
                        if (descriptor != null) services.Remove(descriptor);

                        // Add our Testcontainer db
                        services.AddDbContext<ReportsVoteDbContext>(options =>
                            options.UseNpgsql(connectionString));

                        // Override kafka settings etc
                        // Wait, since we are doing basic check, if we don't mock it completely, we should supply kafka config
                    });
                });

            // Make sure the DB is created
            using (var scope = factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ReportsVoteDbContext>();
                await dbContext.Database.EnsureCreatedAsync();
            }

            var client = factory.CreateClient();

            var newReport = new Report
            {
                UserID = "user-test",
                Description = "A huge pothole in test",
                Category = PotholeCategory.Pothole,
                Status = ReportStatus.Reported,
                Latitude = 48.5,
                LongTitude = 17.5
            };

            var content = new StringContent(JsonSerializer.Serialize(newReport), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/api/Reports", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            using (var scope = factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ReportsVoteDbContext>();
                var reportsInDb = await dbContext.Reports.ToListAsync();
                
                reportsInDb.Should().HaveCount(1);
                reportsInDb[0].Description.Should().Be("A huge pothole in test");
            }
        }
    }
}
