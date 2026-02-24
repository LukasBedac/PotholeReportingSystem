CREATE DATABASE IF NOT EXISTS admin_schema;
CREATE DATABASE IF NOT EXISTS reports_schema;
CREATE DATABASE IF NOT EXISTS auth_schema;

GRANT ALL PRIVILEGES ON admin_schema.* TO 'shared_db_user'@'%';
GRANT ALL PRIVILEGES ON reports_schema.* TO 'shared_db_user'@'%';
GRANT ALL PRIVILEGES ON auth_schema.* TO 'shared_db_user'@'%';
FLUSH PRIVILEGES;
