using AdminService.Database;
using AdminService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminsController : ControllerBase
    {
        private readonly AdminDbContext _context;

        public AdminsController(AdminDbContext context)
        {
            _context = context;
        }

        [HttpGet("by-district")]
        public async Task<ActionResult<Admin>> GetAdminByDistrict([FromQuery] string district)
        {
            if (string.IsNullOrWhiteSpace(district))
                return BadRequest("District is required");

            // Look for an admin assigned to this district
            // TODO - logic update for cities and regions
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.AssignedDistrict == district);

            if (admin == null)
                return NotFound("No admin found for this district");

            return Ok(admin);
        }

        [HttpPost("assign-district")]
        public async Task<ActionResult> AssignDistrict([FromBody] AssignDistrictDto dto)
        {
            if (dto.AdminId == Guid.Empty || string.IsNullOrWhiteSpace(dto.District))
                return BadRequest();

            var admin = await _context.Admins.FindAsync(dto.AdminId);
            if (admin == null)
            {
                // If admin doesn't exist in local DB (but exists in Keycloak), create it
                admin = new Admin { ID = dto.AdminId };
                _context.Admins.Add(admin);
            }

            admin.AssignedDistrict = dto.District;
            await _context.SaveChangesAsync();

            return Ok(admin);
        }

        [HttpPost("{id}/assign-report")]
        public async Task<ActionResult> AssignReport(Guid id, [FromBody] Guid reportId)
        {
            if (reportId == Guid.Empty)
                return BadRequest("ReportId is required");

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound("Admin not found");
            }

            if (!admin.AssignedReports.Contains(reportId))
            {
                admin.AssignedReports.Add(reportId);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        public class AssignDistrictDto
        {
            public Guid AdminId { get; set; }
            public string District { get; set; }
        }
    }
}
