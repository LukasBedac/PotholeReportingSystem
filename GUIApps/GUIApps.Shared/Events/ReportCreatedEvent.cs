namespace GUIApps.Shared.Events
{
    public class ReportCreatedEvent
    {
        public Guid ReportId { get; set; }
        public string UserID { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public DateTime ReportedAt { get; set; }
    }
}
