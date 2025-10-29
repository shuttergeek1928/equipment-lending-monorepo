# Equipment Lending Monorepo

This repo contains a monorepo skeleton for the Equipment Lending assignment:
- React frontend
- Java services: authentication & equipment management
- .NET services: borrowing & returns, dashboard
- PostgreSQL for data

## Quick start (dev)
1. Copy `.env.example` -> `.env` and adjust if needed.
2. Run `./scripts/start-all.sh`
3. Open services:
   - Frontend: http://localhost:3000
   - Java Auth: http://localhost:8081
   - Java Equipment: http://localhost:8082
   - .NET Borrow: http://localhost:8083
   - .NET Dashboard: http://localhost:8084

## Next steps
- Generate Spring Boot projects inside `services/java-*`
- Generate ASP.NET projects inside `services/dotnet-*`
- Implement Dockerfiles build steps and OpenAPI docs