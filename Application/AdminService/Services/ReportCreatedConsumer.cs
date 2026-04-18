using Confluent.Kafka;
using GUIApps.Shared.Events;
using GUIApps.Shared.Services.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using AdminService.Database;
using Microsoft.EntityFrameworkCore;

namespace AdminService.Services
{
    public class ReportCreatedConsumer : BackgroundService
    {
        private readonly string _bootstrapServers;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReportCreatedConsumer> _logger;

        public ReportCreatedConsumer(string bootstrapServers, IServiceProvider serviceProvider, ILogger<ReportCreatedConsumer> logger)
        {
            _bootstrapServers = bootstrapServers;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield(); // Free the caller

            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = "admin-service-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe("report-created-topic");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    if (consumeResult?.Message != null)
                    {
                        var eventMsg = JsonSerializer.Deserialize<ReportCreatedEvent>(consumeResult.Message.Value);
                        if (eventMsg != null)
                        {
                            _logger.LogInformation("Consumed ReportCreatedEvent for Report: {ReportId}", eventMsg.ReportId);
                            await ProcessEventAsync(eventMsg);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break; // expected
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error consuming kafka message");
                }
            }
            consumer.Close();
        }

        private async Task ProcessEventAsync(ReportCreatedEvent cmd)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AdminDbContext>();
            var kafkaProducer = scope.ServiceProvider.GetRequiredService<IKafkaProducer>();

            var region = !string.IsNullOrWhiteSpace(cmd.Region) ? cmd.Region : cmd.District;
            if (string.IsNullOrEmpty(region)) return;

            var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.AssignedDistrict == region);
            if (admin != null)
            {
                // Update internal DB if needed
                if (!admin.AssignedReports.Contains(cmd.ReportId))
                {
                    admin.AssignedReports.Add(cmd.ReportId);
                    await dbContext.SaveChangesAsync();
                }

                _logger.LogInformation("Assigned Admin {AdminId} to Report {ReportId}. Publishing event...", admin.ID, cmd.ReportId);
                
                var adminAssignedEvent = new AdminAssignedEvent
                {
                    ReportId = cmd.ReportId,
                    AdminId = admin.ID.ToString()
                };

                await kafkaProducer.ProduceAsync("admin-assigned-topic", cmd.ReportId.ToString(), adminAssignedEvent);
            }
        }
    }
}