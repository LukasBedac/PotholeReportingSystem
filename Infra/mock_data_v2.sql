-- Use the admin schema
USE admin_schema;

-- Insert Admins for each Region
-- REPLACE_WITH_UUID_X should be replaced with actual Keycloak User IDs.

-- 1. Bratislavský kraj
INSERT INTO Admins (ID, AssignedDistrict, AssignedReports) 
VALUES ('25c32da3-76fc-4f82-91c3-08bfe11adfd1', 'Bratislavský kraj', '[]');

-- 2. Trnavský kraj
INSERT INTO Admins (ID, AssignedDistrict, AssignedReports) 
VALUES ('9eee9fa0-6c68-4ed8-ab8f-ad834a5ce90e', 'Trnavský kraj', '[]');

-- 3. Trenèiansky kraj
INSERT INTO Admins (ID, AssignedDistrict, AssignedReports) 
VALUES ('4a5fd863-75b2-46d0-af4f-79a2beeefc4b', 'Trenèiansky kraj', '[]');

-- 4. Nitriansky kraj
INSERT INTO Admins (ID, AssignedDistrict, AssignedReports) 
VALUES ('49d3e8e5-9d7e-4e7c-ad88-7a5cd0dc55ee', 'Nitriansky kraj', '[]');

-- 5. Žilinský kraj
INSERT INTO Admins (ID, AssignedDistrict, AssignedReports) 
VALUES ('ac3f9f46-91d8-4147-bae1-fafc832a523a', 'Žilinský kraj', '[]');

-- 6. Banskobystrický kraj
INSERT INTO Admins (ID, AssignedDistrict, AssignedReports) 
VALUES ('96a33501-8627-4ead-9688-08744302ac0e', 'Banskobystrický kraj', '[]');

-- 7. Prešovský kraj
INSERT INTO Admins (ID, AssignedDistrict, AssignedReports) 
VALUES ('344c58fb-6d3f-4e0d-87ee-4d95e6022741', 'Prešovský kraj', '[]');

-- 8. Košický kraj
INSERT INTO Admins (ID, AssignedDistrict, AssignedReports) 
VALUES ('3ebfb4da-4abb-421e-8f19-0af02dc7f834', 'Košický kraj', '[]');


-- Use the reports schema
USE reports_schema;

-- Mock Reports (20 entries)
-- User IDs: 
-- Lukas: c9a9af27-7daf-4e1c-bb52-1393e625a95f
-- TestUser: 4f906d78-f602-4344-9cb2-d763c60515cf

-- 1. Bratislava (Bratislavský kraj) -> UUID_1
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 48.1486, 17.1077, 'Deep pothole near the castle gate.', 'Bratislava', 'Bratislava I', 'Bratislavský kraj', NOW() - INTERVAL 10 DAY, 0, 0, NOW(), 'REPLACE_WITH_UUID_1');

-- 2. Trnava (Trnavský kraj) -> UUID_2
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 48.3709, 17.5873, 'Cracked asphalt on the main square.', 'Trnava', 'Trnava', 'Trnavský kraj', NOW() - INTERVAL 9 DAY, 0, 1, NOW(), 'REPLACE_WITH_UUID_2');

-- 3. Trenèín (Trenèiansky kraj) -> UUID_3
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 48.8945, 18.0444, 'Pothole damaging suspensions.', 'Trenèín', 'Trenèín', 'Trenèiansky kraj', NOW() - INTERVAL 8 DAY, 1, 0, NOW(), 'REPLACE_WITH_UUID_3');

-- 4. Nitra (Nitriansky kraj) -> UUID_4
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 48.3061, 18.0764, 'Small hole near the university.', 'Nitra', 'Nitra', 'Nitriansky kraj', NOW() - INTERVAL 7 DAY, 0, 0, NOW(), 'REPLACE_WITH_UUID_4');

-- 5. Žilina (Žilinský kraj) -> UUID_5
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 49.2231, 18.7394, 'Dangerous hole on the bridge.', 'Žilina', 'Žilina', 'Žilinský kraj', NOW() - INTERVAL 6 DAY, 2, 0, NOW(), 'REPLACE_WITH_UUID_5');

-- 6. Banská Bystrica (Banskobystrický kraj) -> UUID_6
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 48.7363, 19.1462, 'Missing manhole cover.', 'Banská Bystrica', 'Banská Bystrica', 'Banskobystrický kraj', NOW() - INTERVAL 5 DAY, 0, 4, NOW(), 'REPLACE_WITH_UUID_6');

-- 7. Prešov (Prešovský kraj) -> UUID_7
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 49.0018, 21.2393, 'Large pothole in the residential area.', 'Prešov', 'Prešov', 'Prešovský kraj', NOW() - INTERVAL 4 DAY, 0, 0, NOW(), 'REPLACE_WITH_UUID_7');

-- 8. Košice (Košický kraj) -> UUID_8
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 48.7164, 21.2611, 'Road degradation near the station.', 'Košice', 'Košice I', 'Košický kraj', NOW() - INTERVAL 3 DAY, 0, 1, NOW(), 'REPLACE_WITH_UUID_8');

-- 9. Senec (Bratislavský kraj) -> UUID_1
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 48.2195, 17.4004, 'Small pothole.', 'Senec', 'Senec', 'Bratislavský kraj', NOW() - INTERVAL 12 DAY, 3, 0, NOW(), 'REPLACE_WITH_UUID_1');

-- 10. Piešany (Trnavský kraj) -> UUID_2
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 48.5914, 17.8306, 'Medium pothole near spa.', 'Piešany', 'Piešany', 'Trnavský kraj', NOW() - INTERVAL 11 DAY, 0, 0, NOW(), 'REPLACE_WITH_UUID_2');

-- 11. Prievidza (Trenèiansky kraj) -> UUID_3
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 48.7716, 18.6225, 'Large pothole blocking lane.', 'Prievidza', 'Prievidza', 'Trenèiansky kraj', NOW() - INTERVAL 20 DAY, 2, 0, NOW(), 'REPLACE_WITH_UUID_3');

-- 12. Levice (Nitriansky kraj) -> UUID_4
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 48.2157, 18.6074, 'Small cracks.', 'Levice', 'Levice', 'Nitriansky kraj', NOW() - INTERVAL 15 DAY, 0, 1, NOW(), 'REPLACE_WITH_UUID_4');

-- 13. Martin (Žilinský kraj) -> UUID_5
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 49.0650, 18.9226, 'Deep hole.', 'Martin', 'Martin', 'Žilinský kraj', NOW() - INTERVAL 2 DAY, 1, 0, NOW(), 'REPLACE_WITH_UUID_5');

-- 14. Zvolen (Banskobystrický kraj) -> UUID_6
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 48.5744, 19.1532, 'Pothole near castle.', 'Zvolen', 'Zvolen', 'Banskobystrický kraj', NOW() - INTERVAL 1 DAY, 0, 0, NOW(), 'REPLACE_WITH_UUID_6');

-- 15. Poprad (Prešovský kraj) -> UUID_7
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 49.0561, 20.2961, 'Large hole near Tatras road.', 'Poprad', 'Poprad', 'Prešovský kraj', NOW(), 0, 2, NOW(), 'REPLACE_WITH_UUID_7');

-- 16. Michalovce (Košický kraj) -> UUID_8
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 48.7554, 21.9189, 'Bad road condition.', 'Michalovce', 'Michalovce', 'Košický kraj', NOW(), 0, 1, NOW(), 'REPLACE_WITH_UUID_8');

-- 17. Malacky (Bratislavský kraj) -> UUID_1
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 48.4363, 17.0210, 'Hole in the road.', 'Malacky', 'Malacky', 'Bratislavský kraj', NOW() - INTERVAL 5 DAY, 0, 0, NOW(), 'REPLACE_WITH_UUID_1');

-- 18. Dunajská Streda (Trnavský kraj) -> UUID_2
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 47.9944, 17.6185, 'Asphalt missing.', 'Dunajská Streda', 'Dunajská Streda', 'Trnavský kraj', NOW() - INTERVAL 4 DAY, 0, 1, NOW(), 'REPLACE_WITH_UUID_2');

-- 19. Partizánske (Trenèiansky kraj) -> UUID_3
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), 'c9a9af27-7daf-4e1c-bb52-1393e625a95f', 48.6272, 18.3756, 'Big pothole.', 'Partizánske', 'Partizánske', 'Trenèiansky kraj', NOW() - INTERVAL 3 DAY, 0, 2, NOW(), 'REPLACE_WITH_UUID_3');

-- 20. Komárno (Nitriansky kraj) -> UUID_4
INSERT INTO Reports (Id, UserID, Latitude, LongTitude, Description, City, District, Region, ReportedAt, Status, Category, LastUpdatedAt, AssignedAdminID)
VALUES (UUID(), '4f906d78-f602-4344-9cb2-d763c60515cf', 47.7661, 18.1272, 'Road needs repair.', 'Komárno', 'Komárno', 'Nitriansky kraj', NOW() - INTERVAL 2 DAY, 0, 0, NOW(), 'REPLACE_WITH_UUID_4');
