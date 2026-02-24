using Microsoft.EntityFrameworkCore;
using GUIApps.Shared.Models;

namespace ReportVoteService.Database
{
    public class ReportsVoteDbContext : DbContext
    {
        public ReportsVoteDbContext(DbContextOptions<ReportsVoteDbContext> options) : base(options) { }

        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure FileMinio as an owned entity if simpler, or just relationships. 
            // Since Report has Image, and Image has ReportId, it's 1:1.
            // TODO - change relation to 1:N
            modelBuilder.Entity<Report>()
                .HasOne(r => r.Image)
                .WithOne(i => i.PotholeReport)
                .HasForeignKey<FileMinio>(i => i.PotholeReportId);
        }
    }
}
