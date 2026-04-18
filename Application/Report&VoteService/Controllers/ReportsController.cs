using Microsoft.AspNetCore.Mvc;
using ReportVoteService.Database;
using GUIApps.Shared.Models;
using Microsoft.EntityFrameworkCore;
using GUIApps.Shared.Events;
using GUIApps.Shared.Services.Kafka;

namespace ReportVoteService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ReportsVoteDbContext _context;
        private readonly ILogger<ReportsController> _logger;
        private readonly IKafkaProducer _kafkaProducer;
        private readonly IHttpClientFactory _httpClientFactory;

        public ReportsController(ReportsVoteDbContext context, ILogger<ReportsController> logger, IKafkaProducer kafkaProducer, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _kafkaProducer = kafkaProducer;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] Report report)
        {
            _logger.LogInformation("Received request to create report for User: {UserID}", report?.UserID);

            if (report == null)
            {
                _logger.LogWarning("CreateReport: Report object is null");
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(report.UserID))
            {
                _logger.LogWarning("CreateReport: UserID is empty");
                return BadRequest("UserID is required");
            }

            // This should't happen unless someone is explicitly setting Id to Guid.Empty
            if (report.Id == Guid.Empty)
                report.Id = Guid.NewGuid();

            report.ReportedAt = DateTime.UtcNow;
            report.LastUpdatedAt = DateTime.UtcNow;

            if (report.Image != null)
            {
                if (report.Image.Id == Guid.Empty)
                    report.Image.Id = Guid.NewGuid();
                report.Image.PotholeReportId = report.Id;
                if (report.Image.UploadedAt == default)
                    report.Image.UploadedAt = DateTime.UtcNow;
            }

            // Persist the report in 'Pending' state, do NOT wait for API HTTP call to AdminService.
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            // Fire and forget via Kafka Events Let the Admin Service handle assignment asynchronously.
            var region = !string.IsNullOrWhiteSpace(report.Region) ? report.Region : 
                         (!string.IsNullOrWhiteSpace(report.District) ? report.District : null);

            var reportCreatedEvent = new ReportCreatedEvent
            {
                ReportId = report.Id,
                UserID = report.UserID,
                Region = region,
                District = report.District,
                ReportedAt = report.ReportedAt
            };

            await _kafkaProducer.ProduceAsync("report-created-topic", report.Id.ToString(), reportCreatedEvent);

            return CreatedAtAction(nameof(GetReport), new { id = report.Id }, report);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReport(Guid id)
        {
            var report = await _context.Reports
                .Include(r => r.Image)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null)
                return NotFound();

            return Ok(report);
        }

        [HttpGet]
        public async Task<IActionResult> GetReports()
        {
            var reports = await _context.Reports
                .Include(r => r.Image)
                .OrderByDescending(r => r.ReportedAt)
                .ToListAsync();

            return Ok(reports);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetReportsByUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("User ID is required");

            var reports = await _context.Reports
                .Include(r => r.Image)
                .Where(r => r.UserID == userId)
                .OrderByDescending(r => r.ReportedAt)
                .ToListAsync();

            return Ok(reports);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(Guid id)
        {
            var report = await _context.Reports
                .Include(r => r.Image)
                .FirstOrDefaultAsync(r => r.Id == id); // Use FirstOrDefaultAsync with Include to get Image data

            if (report == null)
            {
                return NotFound();
            }

            if (report.Image != null && !string.IsNullOrEmpty(report.Image.StorageKey) && !string.IsNullOrEmpty(report.Image.BucketName))
            {
                try 
                {
                    var fileClient = _httpClientFactory.CreateClient("FileService");
                    var deleteUrl = $"Files/delete/{report.Image.BucketName}/{report.Image.StorageKey}";
                    var response = await fileClient.DeleteAsync(deleteUrl);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        _logger.LogWarning("Failed to delete image for report {ReportId}: {StatusCode} - {Error}", id, response.StatusCode, error);
                    }
                    else
                    {
                        _logger.LogInformation("Successfully deleted image for report {ReportId}", id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception deleting image for report {ReportId}", id);
                }
            }

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(Guid id, [FromBody] Report updatedReport)
        {
            if (id != updatedReport.Id)
            {
                return BadRequest("ID mismatch");
            }

            var report = await _context.Reports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            report.Status = updatedReport.Status;
            report.Category = updatedReport.Category;
            report.Description = updatedReport.Description;
            report.LastUpdatedAt = DateTime.UtcNow;
            
            if (!string.IsNullOrWhiteSpace(updatedReport.AssignedAdminID))
            {
                report.AssignedAdminID = updatedReport.AssignedAdminID;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Reports.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
    }
}
