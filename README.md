# OaigLoan

Clean-architecture starter for a loan platform with .NET 9, React, and CQRS/MediatR. Current scope covers JWT authentication and user registration backed by PostgreSQL and EF Core with an outbox table for future event delivery.

## Projects

- `src/Api` - ASP.NET Core API exposing auth endpoints, validation pipeline, JWT issuance.
- `src/Application` - CQRS application layer (commands, DTOs, validators), AutoMapper, MediatR.
- `src/Domain` - Aggregates, domain events, and outbox message model.
- `src/Infrastructure` - EF Core PostgreSQL persistence, repositories, JWT generator, password hashing.
- `frontend` - Vite + React (TypeScript) sample UI that calls the API auth endpoints directly.

## Getting started

1. Ensure PostgreSQL is running and update connection strings in `src/Api/appsettings*.json`.
2. Run the API:  
   - `dotnet run --project src/Api`
3. Frontend: `cd frontend && npm install && npm run dev` (set `VITE_API_BASE_URL` if the API port differs; defaults to `http://localhost:5080`).

The API exposes `/auth/register` and `/auth/login`, returning JWTs and user details.

