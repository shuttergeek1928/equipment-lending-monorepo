User Auth Service - School Lending Portal

This service is one of the two microservices for the School Lending Portal.

Its sole responsibility is to manage user identity:

User registration (signup)

User authentication (login)

Generation of JSON Web Tokens (JWT)

This service is the "token issuer" or "identity provider" for the entire system.

Tech Stack

Java 17

Spring Boot 3.x

Spring Security 6.x: For password encoding and authentication management.

Spring Data JPA / Hibernate: For database interaction.

PostgreSQL: The database for storing users and roles.

JJWT (Java JWT): For creating and signing JWTs.

Maven: For dependency management.

Prerequisites

JDK 17 or newer installed.

Maven installed.

A running PostgreSQL instance.

Setup & Running

Create the Database:
Before starting, create the database in PostgreSQL:

CREATE DATABASE auth_db;


Configure application.properties:
Open src/main/resources/application.properties and update the database credentials:

spring.datasource.url=jdbc:postgresql://localhost:5432/auth_db
spring.datasource.username=your_postgres_username
spring.datasource.password=your_postgres_password


Database Initialization:

The spring.jpa.hibernate.ddl-auto=update property will automatically create the users, roles, and user_roles tables based on the User.java and Role.java entities.

The spring.sql.init.mode=always property will run src/main/resources/data.sql on every startup. This script is responsible for populating the roles table (e.g., with ROLE_STUDENT, ROLE_ADMIN).

Run the Application:
Use the Maven Spring Boot plugin to run the service:

mvn spring-boot:run


The service will start on http://localhost:8081.

Security

This service's security configuration (WebSecurityConfig.java) is set to:

Permit all requests to /api/auth/** (for login and signup).

Block all other requests (as there are none).

The JWT token it generates includes the userId (UUID), roles (Array), and sub (username) as claims.

API Endpoints

(For full details, see the complete api_docs.md)

POST /api/auth/signup

Description: Registers a new user.

Body: SignupRequest object (username, email, password, role array).

POST /api/auth/login

Description: Authenticates a user.

Body: LoginRequest object (username, password).

Success Response: JwtResponse object (id, username, roles, token).