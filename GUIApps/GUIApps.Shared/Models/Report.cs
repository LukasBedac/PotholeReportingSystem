using GUIApps.Shared.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GUIApps.Shared.Models
{
    public class Report
    {
        
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string UserID { get; set; }
        public double Latitude{ get; set; }
        public double LongTitude { get; set; }
        public string? Description { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Region { get; set; }
        public DateTime ReportedAt { get; set; }
        public ReportStatus Status { get; set; }
        public PotholeCategory Category { get; set; }
        public string? AssignedAdminID { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public FileMinio Image { get; set; }
    }
}
