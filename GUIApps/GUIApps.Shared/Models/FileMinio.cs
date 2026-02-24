using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GUIApps.Shared.Models
{
    public class FileMinio
    {
        public Guid Id { get; set; }
        public string OriginalName { get; set; } = string.Empty;
        public string StorageKey { get; set; } = string.Empty;
        public string BucketName { get; set; } = "reports";
        public string ContentType { get; set; } = string.Empty; // e.g. image/jpeg
        public long SizeBytes { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public Guid PotholeReportId { get; set; }

        [JsonIgnore]
        public Report? PotholeReport { get; set; }

    }
}
