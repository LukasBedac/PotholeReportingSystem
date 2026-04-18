using GUIApps.Shared.Models;
using GUIApps.Shared.Models.Enums;
using System.Net.Http.Json;

namespace ReportVoteService.Database
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ReportsVoteDbContext context, IHttpClientFactory httpClientFactory)
        {
            if (context.Reports.Any())
            {
                return;
            }

            var client = httpClientFactory.CreateClient("FileService");
            var lukasId = "c9a9af27-7daf-4e1c-bb52-1393e625a95f";
            var testUserId = "4f906d78-f602-4344-9cb2-d763c60515cf";
            
            var adminMap = new Dictionary<string, string>
            {
                { "Bratislavsk˝ kraj", "25c32da3-76fc-4f82-91c3-08bfe11adfd1" },
                { "Trnavsk˝ kraj", "9eee9fa0-6c68-4ed8-ab8f-ad834a5ce90e" },
                { "TrenËiansky kraj", "4a5fd863-75b2-46d0-af4f-79a2beeefc4b" },
                { "Nitriansky kraj", "49d3e8e5-9d7e-4e7c-ad88-7a5cd0dc55ee" },
                { "éilinsk˝ kraj", "ac3f9f46-91d8-4147-bae1-fafc832a523a" },
                { "Banskobystrick˝ kraj", "96a33501-8627-4ead-9688-08744302ac0e" },
                { "Preöovsk˝ kraj", "344c58fb-6d3f-4e0d-87ee-4d95e6022741" },
                { "Koöick˝ kraj", "3ebfb4da-4abb-421e-8f19-0af02dc7f834" }
            };

            var reports = new List<Report>
            {
                // 1
                new Report { 
                    UserID = lukasId, Latitude = 48.1486, LongTitude = 17.1077, 
                    Description = "Deep pothole near the castle gate.", City = "Bratislava", District = "Bratislava I", Region = "Bratislavsk˝ kraj", 
                    ReportedAt = DateTime.UtcNow.AddDays(-10), Status = ReportStatus.Reported, Category = PotholeCategory.CrackedRoad, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["Bratislavsk˝ kraj"]
                },
                // 2
                new Report {
                    UserID = testUserId, Latitude = 48.3709, LongTitude = 17.5873,
                    Description = "Cracked asphalt on the main square.", City = "Trnava", District = "Trnava", Region = "Trnavsk˝ kraj",
                    ReportedAt = DateTime.UtcNow.AddDays(-9), Status = ReportStatus.Reported, Category = PotholeCategory.Pothole, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["Trnavsk˝ kraj"]
                },
                // 3
                new Report {
                    UserID = lukasId, Latitude = 48.8945, LongTitude = 18.0444,
                    Description = "Pothole damaging suspensions.", City = "TrenËÌn", District = "TrenËÌn", Region = "TrenËiansky kraj",
                    ReportedAt = DateTime.UtcNow.AddDays(-8), Status = ReportStatus.UnderReview, Category = PotholeCategory.DamagedShoulder, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["TrenËiansky kraj"]
                },
                // 4
                new Report {
                    UserID = testUserId, Latitude = 48.3061, LongTitude = 18.0764,
                    Description = "Small hole near the university.", City = "Nitra", District = "Nitra", Region = "Nitriansky kraj",
                    ReportedAt = DateTime.UtcNow.AddDays(-7), Status = ReportStatus.Reported, Category = PotholeCategory.Pothole, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["Nitriansky kraj"]
                },
                // 5
                new Report {
                    UserID = lukasId, Latitude = 49.2231, LongTitude = 18.7394,
                    Description = "Dangerous hole on the bridge.", City = "éilina", District = "éilina", Region = "éilinsk˝ kraj",
                    ReportedAt = DateTime.UtcNow.AddDays(-6), Status = ReportStatus.Inprogress, Category = PotholeCategory.DamagedShoulder, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["éilinsk˝ kraj"]
                },
                // 6
                new Report {
                    UserID = testUserId, Latitude = 48.7363, LongTitude = 19.1462,
                    Description = "Missing manhole cover.", City = "Bansk· Bystrica", District = "Bansk· Bystrica", Region = "Banskobystrick˝ kraj",
                    ReportedAt = DateTime.UtcNow.AddDays(-5), Status = ReportStatus.Reported, Category = PotholeCategory.Other, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["Banskobystrick˝ kraj"]
                },
                // 7
                new Report {
                    UserID = lukasId, Latitude = 49.0018, LongTitude = 21.2393,
                    Description = "Large pothole in the residential area.", City = "Preöov", District = "Preöov", Region = "Preöovsk˝ kraj",
                    ReportedAt = DateTime.UtcNow.AddDays(-4), Status = ReportStatus.Reported, Category = PotholeCategory.CrackedRoad, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["Preöovsk˝ kraj"]
                },
                // 8
                new Report {
                    UserID = testUserId, Latitude = 48.7164, LongTitude = 21.2611,
                    Description = "Road degradation near the station.", City = "Koöice", District = "Koöice I", Region = "Koöick˝ kraj",
                    ReportedAt = DateTime.UtcNow.AddDays(-3), Status = ReportStatus.Reported, Category = PotholeCategory.CrackedRoad, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["Koöick˝ kraj"]
                },
                // 9
                new Report {
                    UserID = lukasId, Latitude = 48.2195, LongTitude = 17.4004,
                    Description = "Small pothole.", City = "Senec", District = "Senec", Region = "Bratislavsk˝ kraj",
                    ReportedAt = DateTime.UtcNow.AddDays(-12), Status = ReportStatus.Repaired, Category = PotholeCategory.Pothole, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["Bratislavsk˝ kraj"]
                },
                // 10
                new Report {
                    UserID = testUserId, Latitude = 48.5914, LongTitude = 17.8306,
                    Description = "Medium pothole near spa.", City = "Pieöùany", District = "Pieöùany", Region = "Trnavsk˝ kraj",
                    ReportedAt = DateTime.UtcNow.AddDays(-11), Status = ReportStatus.Reported, Category = PotholeCategory.CrackedRoad, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["Trnavsk˝ kraj"]
                },
                // 11
                new Report {
                    UserID = lukasId, Latitude = 48.7716, LongTitude = 18.6225,
                    Description = "Large pothole blocking lane.", City = "Prievidza", District = "Prievidza", Region = "TrenËiansky kraj",
                    ReportedAt = DateTime.UtcNow.AddDays(-20), Status = ReportStatus.Rejected, Category = PotholeCategory.MissingSign, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["TrenËiansky kraj"]
                },
                // 12
                new Report {
                    UserID = testUserId, Latitude = 48.2157, LongTitude = 18.6074,
                    Description = "Small cracks.", City = "Levice", District = "Levice", Region = "Nitriansky kraj",
                    ReportedAt = DateTime.UtcNow.AddDays(-15), Status = ReportStatus.Reported, Category = PotholeCategory.Pothole, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["Nitriansky kraj"]
                },
                // 13
                new Report {
                    UserID = lukasId, Latitude = 49.0650, LongTitude = 18.9226,
                    Description = "Deep hole.", City = "Martin", District = "Martin", Region = "éilinsk˝ kraj",
                    ReportedAt = DateTime.UtcNow.AddDays(-2), Status = ReportStatus.UnderReview, Category = PotholeCategory.CrackedRoad, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["éilinsk˝ kraj"]
                },
                // 14
                new Report {
                    UserID = testUserId, Latitude = 48.5744, LongTitude = 19.1532,
                    Description = "Pothole near castle.", City = "Zvolen", District = "Zvolen", Region = "Banskobystrick˝ kraj",
                    ReportedAt = DateTime.UtcNow.AddDays(-1), Status = ReportStatus.Reported, Category = PotholeCategory.Pothole, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["Banskobystrick˝ kraj"]
                },
                // 15
                new Report {
                    UserID = lukasId, Latitude = 49.0561, LongTitude = 20.2961,
                    Description = "Large hole near Tatras road.", City = "Poprad", District = "Poprad", Region = "Preöovsk˝ kraj",
                    ReportedAt = DateTime.UtcNow, Status = ReportStatus.Reported, Category = PotholeCategory.DamagedShoulder, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["Preöovsk˝ kraj"]
                },
                // 16
                new Report {
                    UserID = testUserId, Latitude = 48.7554, LongTitude = 21.9189,
                    Description = "Bad road condition.", City = "Michalovce", District = "Michalovce", Region = "Koöick˝ kraj",
                    ReportedAt = DateTime.UtcNow, Status = ReportStatus.Reported, Category = PotholeCategory.CrackedRoad, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["Koöick˝ kraj"]
                },
                // 17
                new Report {
                    UserID = lukasId, Latitude = 48.4363, LongTitude = 17.0210,
                    Description = "Hole in the road.", City = "Malacky", District = "Malacky", Region = "Bratislavsk˝ kraj",
                    ReportedAt = DateTime.UtcNow.AddDays(-5), Status = ReportStatus.Reported, Category = PotholeCategory.Pothole, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["Bratislavsk˝ kraj"]
                },
                // 18
                new Report {
                    UserID = testUserId, Latitude = 47.9944, LongTitude = 17.6185,
                    Description = "Asphalt missing.", City = "Dunajsk· Streda", District = "Dunajsk· Streda", Region = "Trnavsk˝ kraj",
                    ReportedAt = DateTime.UtcNow.AddDays(-4), Status = ReportStatus.Reported, Category = PotholeCategory.CrackedRoad, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["Trnavsk˝ kraj"]
                },
                // 19
                new Report {
                    UserID = lukasId, Latitude = 48.6272, LongTitude = 18.3756,
                    Description = "Big pothole.", City = "Partiz·nske", District = "Partiz·nske", Region = "TrenËiansky kraj",
                    ReportedAt = DateTime.UtcNow.AddDays(-3), Status = ReportStatus.Reported, Category = PotholeCategory.DamagedShoulder, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["TrenËiansky kraj"]
                },
                // 20
                new Report {
                    UserID = testUserId, Latitude = 47.7661, LongTitude = 18.1272,
                    Description = "Road needs repair.", City = "Kom·rno", District = "Kom·rno", Region = "Nitriansky kraj",
                    ReportedAt = DateTime.UtcNow.AddDays(-2), Status = ReportStatus.Reported, Category = PotholeCategory.Pothole, LastUpdatedAt = DateTime.UtcNow,
                    AssignedAdminID = adminMap["Nitriansky kraj"]
                }
            };

            // 1x1 Pixel Transparent PNG Base64
            var base64Image = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";
            var imageBytes = Convert.FromBase64String(base64Image);

            foreach (var report in reports)
            {
                try
                {
                    using var content = new MultipartFormDataContent();
                    var fileContent = new ByteArrayContent(imageBytes);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                    content.Add(fileContent, "file", $"seed_report_{report.Id}.png");

                    // Try to upload to FileService
                    // This relies on FileService being up and running.
                    // If running in same docker-compose stack, it might need retries or might fail on first run if FileService is not ready.
                    var response = await client.PostAsync("Files/upload", content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var uploadResult = await response.Content.ReadFromJsonAsync<UploadResponseDto>();
                        if (uploadResult != null)
                        {
                            report.Image = new FileMinio
                            {
                                BucketName = uploadResult.BucketName,
                                StorageKey = uploadResult.StorageKey,
                                OriginalName = uploadResult.OriginalName,
                                ContentType = uploadResult.ContentType,
                                SizeBytes = uploadResult.Size,
                                UploadedAt = DateTime.UtcNow,
                                PotholeReportId = report.Id
                            };
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to seed image for report {report.Id}. Status: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception seeding image for report {report.Id}: {ex.Message}");
                }
            }

            context.Reports.AddRange(reports);
            await context.SaveChangesAsync();
        }

        private class UploadResponseDto
        {
            public string BucketName { get; set; } = "";
            public string StorageKey { get; set; } = "";
            public string OriginalName { get; set; } = "";
            public string ContentType { get; set; } = "";
            public long Size { get; set; }
        }
    }
}
