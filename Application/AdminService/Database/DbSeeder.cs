using AdminService.Models;

namespace AdminService.Database
{
    public static class DbSeeder
    {
        public static void Seed(AdminDbContext context)
        {
            if (context.Admins.Any())
            {
                return;
            }

            var admins = new List<Admin>
            {
                // 1. Bratislavský kraj
                new Admin
                {
                    ID = Guid.Parse("25c32da3-76fc-4f82-91c3-08bfe11adfd1"),
                    AssignedDistrict = "Bratislavský kraj",
                    AssignedReports = new List<Guid>()
                },
                // 2. Trnavský kraj
                new Admin
                {
                    ID = Guid.Parse("9eee9fa0-6c68-4ed8-ab8f-ad834a5ce90e"),
                    AssignedDistrict = "Trnavský kraj",
                    AssignedReports = new List<Guid>()
                },
                // 3. Trenèiansky kraj
                new Admin
                {
                    ID = Guid.Parse("4a5fd863-75b2-46d0-af4f-79a2beeefc4b"),
                    AssignedDistrict = "Trenèiansky kraj",
                    AssignedReports = new List<Guid>()
                },
                // 4. Nitriansky kraj
                new Admin
                {
                    ID = Guid.Parse("49d3e8e5-9d7e-4e7c-ad88-7a5cd0dc55ee"),
                    AssignedDistrict = "Nitriansky kraj",
                    AssignedReports = new List<Guid>()
                },
                // 5. Žilinský kraj
                new Admin
                {
                    ID = Guid.Parse("ac3f9f46-91d8-4147-bae1-fafc832a523a"),
                    AssignedDistrict = "Žilinský kraj",
                    AssignedReports = new List<Guid>()
                },
                // 6. Banskobystrický kraj
                new Admin
                {
                    ID = Guid.Parse("96a33501-8627-4ead-9688-08744302ac0e"),
                    AssignedDistrict = "Banskobystrický kraj",
                    AssignedReports = new List<Guid>()
                },
                // 7. Prešovský kraj
                new Admin
                {
                    ID = Guid.Parse("344c58fb-6d3f-4e0d-87ee-4d95e6022741"),
                    AssignedDistrict = "Prešovský kraj",
                    AssignedReports = new List<Guid>()
                },
                // 8. Košický kraj
                new Admin
                {
                    ID = Guid.Parse("3ebfb4da-4abb-421e-8f19-0af02dc7f834"),
                    AssignedDistrict = "Košický kraj",
                    AssignedReports = new List<Guid>()
                }
            };

            context.Admins.AddRange(admins);
            context.SaveChanges();
        }
    }
}
