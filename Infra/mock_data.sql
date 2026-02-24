-- Use the admin schema
USE admin_schema;

-- Insert Admins for each Region
-- REPLACE_WITH_UUID_X should be replaced with actual UUIDs if you have them, 
-- or you can generate them using UUID() if running this once.
-- Ideally, copy-paste your Keycloak user IDs here.

-- 1. Bratislavsk˝ kraj
INSERT INTO Admins (ID, AssignedDistrict, AssignedReports) 
VALUES ('25c32da3-76fc-4f82-91c3-08bfe11adfd1', 'Bratislavsk˝ kraj', '[]');

-- 2. Trnavsk˝ kraj
INSERT INTO Admins (ID, AssignedDistrict, AssignedReports) 
VALUES ('9eee9fa0-6c68-4ed8-ab8f-ad834a5ce90e', 'Trnavsk˝ kraj', '[]');

-- 3. TrenËiansky kraj
INSERT INTO Admins (ID, AssignedDistrict, AssignedReports) 
VALUES ('4a5fd863-75b2-46d0-af4f-79a2beeefc4b', 'TrenËiansky kraj', '[]');

-- 4. Nitriansky kraj
INSERT INTO Admins (ID, AssignedDistrict, AssignedReports) 
VALUES ('49d3e8e5-9d7e-4e7c-ad88-7a5cd0dc55ee', 'Nitriansky kraj', '[]');

-- 5. éilinsk˝ kraj
INSERT INTO Admins (ID, AssignedDistrict, AssignedReports) 
VALUES ('ac3f9f46-91d8-4147-bae1-fafc832a523a', 'éilinsk˝ kraj', '[]');

-- 6. Banskobystrick˝ kraj
INSERT INTO Admins (ID, AssignedDistrict, AssignedReports) 
VALUES ('96a33501-8627-4ead-9688-08744302ac0e', 'Banskobystrick˝ kraj', '[]');

-- 7. Preöovsk˝ kraj
INSERT INTO Admins (ID, AssignedDistrict, AssignedReports) 
VALUES ('344c58fb-6d3f-4e0d-87ee-4d95e6022741', 'Preöovsk˝ kraj', '[]');

-- 8. Koöick˝ kraj
INSERT INTO Admins (ID, AssignedDistrict, AssignedReports) 
VALUES ('3ebfb4da-4abb-421e-8f19-0af02dc7f834', 'Koöick˝ kraj', '[]');

-- Use the reports schema
USE reports_schema;

-- Mock Reports (20 entries)
-- User IDs: 
-- Lukas: c9a9af27-7daf-4e1c-bb52-1393e625a95f
-- TestUser: 4f906d78-f602-4344-9cb2-d763c60515cf

-- 1. Bratislava (Bratislavsk˝ kraj) - Reported by Lukas
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 48.1486, 17.1077, 'Deep pothole near the castle gate.', 'Bratislava', 'Bratislava I', 'Bratislavsk˝ kraj', NOW() - INTERVAL 10 DAY, 0, 1, NOW());

-- 2. Trnava (Trnavsk˝ kraj) - Reported by TestUser
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 48.3709, 17.5873, 'Cracked asphalt on the main square.', 'Trnava', 'Trnava', 'Trnavsk˝ kraj', NOW() - INTERVAL 9 DAY, 0, 0, NOW());

-- 3. TrenËÌn (TrenËiansky kraj) - Reported by Lukas
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 48.8945, 18.0444, 'Pothole damaging suspensions.', 'TrenËÌn', 'TrenËÌn', 'TrenËiansky kraj', NOW() - INTERVAL 8 DAY, 1, 2, NOW());

-- 4. Nitra (Nitriansky kraj) - Reported by TestUser
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 48.3061, 18.0764, 'Small hole near the university.', 'Nitra', 'Nitra', 'Nitriansky kraj', NOW() - INTERVAL 7 DAY, 0, 0, NOW());

-- 5. éilina (éilinsk˝ kraj) - Reported by Lukas
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 49.2231, 18.7394, 'Dangerous hole on the bridge.', 'éilina', 'éilina', 'éilinsk˝ kraj', NOW() - INTERVAL 6 DAY, 2, 2, NOW());

-- 6. Bansk· Bystrica (Banskobystrick˝ kraj) - Reported by TestUser
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 48.7363, 19.1462, 'Missing manhole cover.', 'Bansk· Bystrica', 'Bansk· Bystrica', 'Banskobystrick˝ kraj', NOW() - INTERVAL 5 DAY, 0, 2, NOW());

-- 7. Preöov (Preöovsk˝ kraj) - Reported by Lukas
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 49.0018, 21.2393, 'Large pothole in the residential area.', 'Preöov', 'Preöov', 'Preöovsk˝ kraj', NOW() - INTERVAL 4 DAY, 0, 1, NOW());

-- 8. Koöice (Koöick˝ kraj) - Reported by TestUser
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 48.7164, 21.2611, 'Road degradation near the station.', 'Koöice', 'Koöice I', 'Koöick˝ kraj', NOW() - INTERVAL 3 DAY, 0, 1, NOW());

-- 9. Senec (Bratislavsk˝ kraj) - Reported by Lukas
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 48.2195, 17.4004, 'Small pothole.', 'Senec', 'Senec', 'Bratislavsk˝ kraj', NOW() - INTERVAL 12 DAY, 3, 0, NOW());

-- 10. Pieöùany (Trnavsk˝ kraj) - Reported by TestUser
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 48.5914, 17.8306, 'Medium pothole near spa.', 'Pieöùany', 'Pieöùany', 'Trnavsk˝ kraj', NOW() - INTERVAL 11 DAY, 0, 1, NOW());

-- 11. Prievidza (TrenËiansky kraj) - Reported by Lukas
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 48.7716, 18.6225, 'Large pothole blocking lane.', 'Prievidza', 'Prievidza', 'TrenËiansky kraj', NOW() - INTERVAL 20 DAY, 4, 3, NOW());

-- 12. Levice (Nitriansky kraj) - Reported by TestUser
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 48.2157, 18.6074, 'Small cracks.', 'Levice', 'Levice', 'Nitriansky kraj', NOW() - INTERVAL 15 DAY, 0, 0, NOW());

-- 13. Martin (éilinsk˝ kraj) - Reported by Lukas
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 49.0650, 18.9226, 'Deep hole.', 'Martin', 'Martin', 'éilinsk˝ kraj', NOW() - INTERVAL 2 DAY, 1, 1, NOW());

-- 14. Zvolen (Banskobystrick˝ kraj) - Reported by TestUser
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 48.5744, 19.1532, 'Pothole near castle.', 'Zvolen', 'Zvolen', 'Banskobystrick˝ kraj', NOW() - INTERVAL 1 DAY, 0, 0, NOW());

-- 15. Poprad (Preöovsk˝ kraj) - Reported by Lukas
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 49.0561, 20.2961, 'Large hole near Tatras road.', 'Poprad', 'Poprad', 'Preöovsk˝ kraj', NOW(), 0, 2, NOW());

-- 16. Michalovce (Koöick˝ kraj) - Reported by TestUser
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 48.7554, 21.9189, 'Bad road condition.', 'Michalovce', 'Michalovce', 'Koöick˝ kraj', NOW(), 0, 1, NOW());

-- 17. Malacky (Bratislavsk˝ kraj) - Reported by Lukas
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 48.4363, 17.0210, 'Hole in the road.', 'Malacky', 'Malacky', 'Bratislavsk˝ kraj', NOW() - INTERVAL 5 DAY, 0, 0, NOW());

-- 18. Dunajsk· Streda (Trnavsk˝ kraj) - Reported by TestUser
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 47.9944, 17.6185, 'Asphalt missing.', 'Dunajsk· Streda', 'Dunajsk· Streda', 'Trnavsk˝ kraj', NOW() - INTERVAL 4 DAY, 0, 1, NOW());

-- 19. Partiz·nske (TrenËiansky kraj) - Reported by Lukas
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 48.6272, 18.3756, 'Big pothole.', 'Partiz·nske', 'Partiz·nske', 'TrenËiansky kraj', NOW() - INTERVAL 3 DAY, 0, 2, NOW());

-- 20. Kom·rno (Nitriansky kraj) - Reported by TestUser
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 47.7661, 18.1272, 'Road needs repair.', 'Kom·rno', 'Kom·rno', 'Nitriansky kraj', NOW() - INTERVAL 2 DAY, 0, 0, NOW());
