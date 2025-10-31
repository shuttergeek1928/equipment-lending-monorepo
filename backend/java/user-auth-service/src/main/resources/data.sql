-- This runs on startup if spring.jpa.hibernate.ddl-auto is 'create' or 'create-drop'
-- Or if spring.sql.init.mode=always is set
INSERT INTO roles(name) VALUES('ROLE_STUDENT') ON CONFLICT DO NOTHING;
INSERT INTO roles(name) VALUES('ROLE_STAFF') ON CONFLICT DO NOTHING;
INSERT INTO roles(name) VALUES('ROLE_ADMIN') ON CONFLICT DO NOTHING;