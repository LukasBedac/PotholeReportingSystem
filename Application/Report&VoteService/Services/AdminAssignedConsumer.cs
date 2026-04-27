using Confluent.Kafka;
using GUIApps.Shared.Events;
using ReportVoteService.Database;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ReportVoteService.Services
{
    public class AdminAssignedConsumer : BackgroundService
    {
        private readonly string _bootstrapServers;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AdminAssignedConsumer> _logger;

        public AdminAssignedConsumer(string bootstrapServers, IServiceProvider serviceProvider, ILogger<AdminAssignedConsumer> logger)
        {
            _bootstrapServers = bootstrapServers;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = "report-service-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe("admin-assigned-topic");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    if (consumeResult?.Message != null)
                    {
                        var eventMsg = JsonSerializer.Deserialize<AdminAssignedEvent>(consumeResult.Message.Value);
                        if (eventMsg != null)
                        {
                            _logger.LogInformation("Consumed AdminAssignedEvent for Report: {ReportId}", eventMsg.ReportId);
                            await UpdateReportAdminAsync(eventMsg);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error consuming admin-assigned-topic");
                }
            }
            consumer.Close();
        }

        public async Task UpdateReportAdminAsync(AdminAssignedEvent msg)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ReportsVoteDbContext>();
            
            var report = await dbContext.Reports.FirstOrDefaultAsync(r => r.Id == msg.ReportId);
            if (report != null)
            {
                report.AssignedAdminID = msg.AdminId;
                await dbContext.SaveChangesAsync();
                _logger.LogInformation("Successfully updated Report {ReportId} with Admin {AdminId}", msg.ReportId, msg.AdminId);
            }
        }
    }
}
