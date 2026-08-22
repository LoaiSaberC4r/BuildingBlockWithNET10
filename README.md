# AcademyOps technical foundation

AcademyOps is a .NET 10 / C# 14 Clean Architecture solution prepared for future product development. Phase 0 is intentionally domain-neutral: it contains no AcademyOps business entities, roles, permissions, tenancy, or use cases.

## Structure

```text
src/
  BuildingBlock/
    BuildingBlock.Api
    BuildingBlock.Application
    BuildingBlock.Domain
    BuildingBlock.Infrastructure
    BuildingBlock.Infrastructure.EntityFrameworkCore.SqlServer
  AcademyOps/
    AcademyOps.Api
    AcademyOps.Application
    AcademyOps.Domain
    AcademyOps.Infrastructure
    AcademyOps.Infrastructure.EntityFrameworkCore.SqlServer

tests/
  AcademyOps.Tests.Unit
  AcademyOps.Tests.Integration
  AcademyOps.Tests.Architecture
```

The reusable BuildingBlock keeps results, CQRS behaviors, repositories, specifications, UnitOfWork, transactions, caching, auditing, soft delete, concurrency, logging, localization, ProblemDetails, email delivery, and SQL Server support.

The application persistence baseline contains only the generic email outbox. Enqueueing does not call `SaveChangesAsync`; the caller owns the transaction so future domain changes and outbox messages can be committed atomically.

## Configuration and secrets

Committed configuration contains local, non-secret defaults only. Override sensitive values with User Secrets, operating-system or deployment environment variables, or a local ignored `.env` file.

```powershell
dotnet user-secrets set "ConnectionStrings:Database" "<connection-string>" `
  --project src/AcademyOps/AcademyOps.Api
```

The default logical database name is `AcademyOpsDb`. Database startup migration and the outbox worker are disabled by default and must be enabled explicitly.

## Build and verify

```powershell
dotnet tool restore
dotnet restore AcademyOps.sln
dotnet build AcademyOps.sln
dotnet test AcademyOps.sln
dotnet tool run dotnet-ef migrations list `
  --project src/AcademyOps/AcademyOps.Infrastructure.EntityFrameworkCore.SqlServer `
  --startup-project src/AcademyOps/AcademyOps.Api `
  --no-connect
```

Do not begin product business modeling in Phase 0.
