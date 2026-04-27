using System;
using System.Threading.Tasks;
using FluentAssertions;
using GUIApps.Shared.Events;
using GUIApps.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ReportVoteService.Database;
using ReportVoteService.Services;
using Xunit;

namespace Application.Tests
{
    public class AdminAssignedConsumerTests : IDisposable
    {
        private readonly ReportsVoteDbContext _dbContext;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<ILogger<AdminAssignedConsumer>> _loggerMock;
        private readonly AdminAssignedConsumer _consumer;

        public AdminAssignedConsumerTests()
        {
            var dbName = Guid.NewGuid().ToString();
            var services = new ServiceCollection();
            services.AddDbContext<ReportsVoteDbContext>(opt => opt.UseInMemoryDatabase(dbName));

            _serviceProvider = services.BuildServiceProvider();
            _dbContext = _serviceProvider.GetRequiredService<ReportsVoteDbContext>();
            _loggerMock = new Mock<ILogger<AdminAssignedConsumer>>();

            _consumer = new AdminAssignedConsumer("mock-broker", _serviceProvider, _loggerMock.Object);
        }

        [Fact]
        public async Task UpdateReportAdminAsync_With_ValidEvent_Should_Update_AssignedAdminID()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var report = new Report 
            { 
                Id = reportId, 
                UserID = "user-1", 
                AssignedAdminID = null,
                Image = new FileMinio { Id = Guid.NewGuid() }
            };
            
            _dbContext.Reports.Add(report);
            await _dbContext.SaveChangesAsync();

            var adminEvent = new AdminAssignedEvent
            {
                ReportId = reportId,
                AdminId = "admin-123"
            };

            // Act
            await _consumer.UpdateReportAdminAsync(adminEvent);

            // Assert
            _dbContext.ChangeTracker.Clear();
            var updatedReport = await _dbContext.Reports.FirstOrDefaultAsync(r => r.Id == reportId);
            updatedReport.Should().NotBeNull();
            updatedReport!.AssignedAdminID.Should().Be("admin-123");
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
            _serviceProvider.Dispose();
        }
    }
}
