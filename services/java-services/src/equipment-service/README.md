Equipment Service - School Lending Portal

This is the second microservice for the School Lending Portal.

Its responsibility is to manage the equipment inventory. It is a Resource Server, meaning it does not issue tokens, but it validates them to protect its endpoints.

Tech Stack

Java 17

Spring Boot 3.x

Spring Security 6.x: For endpoint protection (@PreAuthorize).

Spring Data JPA / Hibernate: For database interaction.

PostgreSQL: The database for storing the equipments table.

JJWT (Java JWT): For parsing and validating incoming JWTs.

Maven: For dependency management.

Prerequisites

JDK 17 or newer installed.

Maven installed.

A running PostgreSQL instance.

The Auth Service must be running to issue tokens.

Setup & Running

Create the Database:
Before starting, create the database in PostgreSQL:

CREATE DATABASE equipment_db;


Configure application.properties:
Open src/main/resources/application.properties and update the database credentials and JWT secret.

spring.datasource.url=jdbc:postgresql://localhost:5432/equipment_db
spring.datasource.username=your_postgres_username
spring.datasource.password=your_postgres_password

# --- CRITICAL ---
# This secret MUST be identical to the one in the 'auth-service'
school.app.jwtSecret=SuperSecretKeyForSchoolLendingJWTs


Database Initialization:

The spring.jpa.hibernate.ddl-auto=update property will automatically create the equipments table based on the Equipment.java entity.

Run the Application:
Use the Maven Spring Boot plugin to run the service:

mvn spring-boot:run


The service will start on http://localhost:8082.

Security

This service's security configuration (WebSecurityConfig.java) is set to:

Permit all GET requests (e.g., /api/equipment and /api/equipment/{id}).

Require a valid JWT token for all other requests.

The AuthTokenFilter validates the token and extracts the user's roles.

The EquipmentController uses @PreAuthorize("hasRole('ADMIN')") to ensure only users with the ROLE_ADMIN authority can POST, PUT, or DELETE equipment.

API Endpoints

(For full details, see the complete api_docs.md)

GET /api/equipment

Security: Public

Description: Gets a list of all equipment.

GET /api/equipment/{id}

Security: Public

Description: Gets a single equipment item by its UUID.

POST /api/equipment

Security: Admin Only (ROLE_ADMIN)

Body: EquipmentCreateDTO object.

Description: Adds a new item to the inventory.

PUT /api/equipment/{id}

Security: Admin Only (ROLE_ADMIN)

Body: EquipmentUpdateDTO object.

Description: Updates an existing item.

DELETE /api/equipment/{id}

Security: Admin Only (ROLE_ADMIN)

Description: Deletes an item from the inventory.