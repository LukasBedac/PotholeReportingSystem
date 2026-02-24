using GUIApps.Shared.Models;

namespace AdminService.Models
{
    public class Admin
    {
        public Guid ID { get; set; }
        public string? AssignedDistrict { get; set; }
        public List<Guid> AssignedReports { get; set; } = new List<Guid>();
    }
}
