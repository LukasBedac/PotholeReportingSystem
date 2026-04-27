using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using GUIApps.Shared.Models;
using GUIApps.Shared.Models.Enums;
using GUIApps.Shared.Services.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ReportVoteService.Controllers;
using ReportVoteService.Database;
using Xunit;

namespace Application.Tests
{
    public class ReportsControllerTests : IDisposable
    {
        private readonly ReportsVoteDbContext _dbContext;
        private readonly Mock<ILogger<ReportsController>> _loggerMock;
        private readonly Mock<IKafkaProducer> _kafkaProducerMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly ReportsController _controller;

        public ReportsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ReportsVoteDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ReportsVoteDbContext(options);
            _loggerMock = new Mock<ILogger<ReportsController>>();
            _kafkaProducerMock = new Mock<IKafkaProducer>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();

            _controller = new ReportsController(
                _dbContext,
                _loggerMock.Object,
                _kafkaProducerMock.Object,
                _httpClientFactoryMock.Object);
        }

        [Fact]
        public async Task GetReports_Should_ReturnOk_With_Reports()
        {
            // Arrange
            var report = new Report
            {
                Id = Guid.NewGuid(),
                UserID = "user-1",
                Category = PotholeCategory.Pothole,
                Description = "A big pothole",
                Latitude = 48.1234,
                LongTitude = 17.5678,
                ReportedAt = DateTime.UtcNow,
                Status = ReportStatus.Reported,
                Image = new FileMinio { Id = Guid.NewGuid() } // To avoid null reference
            };
            _dbContext.Reports.Add(report);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.GetReports();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            var retrievedReports = okResult.Value as List<Report>;

            retrievedReports.Should().NotBeNull();
            retrievedReports.Should().HaveCount(1);
            retrievedReports!.First().UserID.Should().Be("user-1");
        }

        [Fact]
        public async Task CreateReport_With_NullReport_Should_ReturnBadRequest()
        {
            // Act
            var result = await _controller.CreateReport(null!);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task CreateReport_With_MissingUserId_Should_ReturnBadRequest()
        {
            // Arrange
            var invalidReport = new Report { UserID = string.Empty };

            // Act
            var result = await _controller.CreateReport(invalidReport);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)result).Value.Should().Be("UserID is required");
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
