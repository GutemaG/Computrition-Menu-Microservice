# Computrition-Menu-Microservice

.NET 8 ASP.NET Core Web API microservice for managing **patients**, **menu items**, and computing a patient’s **allowed menu** based on dietary restrictions.

This repository is designed to be easy to explain in an interview: it demonstrates a clean separation of concerns (Controller → Service → Repository), unit tests with Moq, and a pragmatic data-access approach using **EF Core** + **Dapper**.

---

## Table of Contents

- Overview
- Tech Stack
- Architecture (How requests flow)
- Multi-tenancy (Hospital tenants)
- Folder Structure
- Domain Model
- API Endpoints (examples)
- Data Access: EF Core vs Dapper (why both)
- Running Locally
- Swagger (how to send X-Tenant-Id)
- Tests
- Common Pitfalls (EF tracking)
- Notes / Improvements

---

## Overview

This service exposes endpoints to:

- Create/read/update/delete **MenuItem** records
- Create/read/update/delete **Patient** records
- Get the **allowed menu** for a patient using their `DietaryRestrictionCode`

Dietary restrictions currently supported (see the `DietaryRestriction` enum):

- `GF` (Gluten Free)
- `SF` (Sugar Free)
- `HH` (Heart Healthy)

---

## Tech Stack

- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core** (ORM + change tracking + simple CRUD)
- **Dapper** (high-performance SQL for read-heavy queries)
- **xUnit** + **Moq** (unit tests)

---

## Architecture (How requests flow)

This project uses a layered structure:

1. **Controllers**
	 - HTTP boundary: routing, status codes, basic request validation.
2. **Services**
	 - Business logic / orchestration (e.g., “allowed menu for patient” involves patient + menu queries).
3. **Repositories**
	 - Data access (EF Core and Dapper).

### Typical request flow

Example: GET allowed menu for a patient

- `PatientsController` calls `IMenuService.GetAllowedMenuForPatientAsync(patientId)`
- `MenuService` loads the patient from `IPatientRepository` and uses the patient’s restriction code to filter menu items via `IMenuRepository`
- Repository executes the query and returns results

This structure makes the service easier to test: the service layer can be unit tested by mocking repositories.

---

## Multi-tenancy (Hospital tenants)

This service supports a simple **header-based** multi-tenancy model where the tenant is a **Hospital**.

### How the tenant is selected

- Every tenant-scoped request must include the header `X-Tenant-Id` with the **hospital slug** (example: `hospital-a`, `hospital-b`).
- The tenant header is enforced by middleware: [Computrition.MenuService.API/MultiTenancy/TenantHeaderMiddleware.cs](Computrition.MenuService.API/MultiTenancy/TenantHeaderMiddleware.cs)
- The middleware resolves the slug to a real `Hospital.Id` using the `Hospitals` table and stores it in a scoped tenant context: [Computrition.MenuService.API/MultiTenancy/ITenantContext.cs](Computrition.MenuService.API/MultiTenancy/ITenantContext.cs) and [Computrition.MenuService.API/MultiTenancy/TenantContext.cs](Computrition.MenuService.API/MultiTenancy/TenantContext.cs)

### Endpoints that do NOT require the header

Hospital endpoints are intentionally public (so you can create/list hospitals without already belonging to one). These endpoints are annotated with:

- `[SkipTenantHeader]` in [Computrition.MenuService.API/Controllers/HospitalController.cs](Computrition.MenuService.API/Controllers/HospitalController.cs)

The attribute definition is:

- [Computrition.MenuService.API/MultiTenancy/SkipTenantHeaderAttribute.cs](Computrition.MenuService.API/MultiTenancy/SkipTenantHeaderAttribute.cs)

### Data isolation (Tenant A cannot see Tenant B)

Tenant isolation is enforced in two places:

1. **EF Core**: global query filters on `Patient` and `MenuItem` by `HospitalId` (see [Computrition.MenuService.API/Data/AppDbContext.cs](Computrition.MenuService.API/Data/AppDbContext.cs)). This makes EF queries automatically tenant-scoped.
2. **Dapper**: all tenant-scoped SQL queries include `WHERE HospitalId = @HospitalId` (see [Computrition.MenuService.API/Repositories/PatientRepository.cs](Computrition.MenuService.API/Repositories/PatientRepository.cs) and [Computrition.MenuService.API/Repositories/MenuRepository.cs](Computrition.MenuService.API/Repositories/MenuRepository.cs)).

### Slug generation

`Hospital.Slug` is generated from `Hospital.Name` using [Computrition.MenuService.API/Utility/Slugify.cs](Computrition.MenuService.API/Utility/Slugify.cs).

The slugification rule is applied automatically by overriding `SaveChanges` / `SaveChangesAsync` in the EF Core context:

- [Computrition.MenuService.API/Data/AppDbContext.cs](Computrition.MenuService.API/Data/AppDbContext.cs)

---

## Folder Structure

```text
.
├── Computrition-Menu-Microservice.sln
├── README.md
├── Computrition.MenuService.API.http
├── Computrition.MenuService.API
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Controllers
│   │   ├── HospitalController.cs
│   │   ├── MenuItemsController.cs
│   │   └── PatientsController.cs
│   ├── Data
│   │   └── AppDbContext.cs
│   ├── Models
│   │   ├── DietaryRestriction.cs
│   │   ├── Hospital.cs
│   │   ├── MenuItem.cs
│   │   └── Patient.cs
│   ├── MultiTenancy
│   │   ├── ITenantContext.cs
│   │   ├── SkipTenantHeaderAttribute.cs
│   │   ├── TenantContext.cs
│   │   └── TenantHeaderMiddleware.cs
│   ├── Repositories
│   │   ├── IHospitalRepository.cs
│   │   ├── IMenuRepository.cs
│   │   ├── IPatientRepository.cs
│   │   ├── HospitalRepository.cs
│   │   ├── MenuRepository.cs
│   │   └── PatientRepository.cs
│   └── Services
│       ├── HospitalService.cs
│       ├── IHospitalService.cs
│       ├── IMenuService.cs
│       ├── IPatientService.cs
│       ├── MenuService.cs
│       └── PatientService.cs
└── Computrition.MenuService.Tests
    ├── Computrition.MenuService.Tests.csproj
    ├── MenuServiceTests.cs
    ├── MultiTenancyControllerTests.cs
    ├── PatientServiceTests.cs
    └── TestInfrastructure
	└── TestWebApplicationFactory.cs
```

### What each area is responsible for

- `Controllers/` → HTTP endpoints and response codes
- `Services/` → business rules, input shaping, cross-repository orchestration
- `Repositories/` → data access (EF Core and/or Dapper)
- `Data/` → EF Core `DbContext` and database mapping
- `Models/` → domain entities / enums
- `Computrition.MenuService.Tests/` → unit tests for the service layer

---

## Domain Model (at a glance)

This service uses **Hospital** as the tenant boundary:

- `Patient.HospitalId` is a foreign key to `Hospital.Id`
- `MenuItem.HospitalId` is a foreign key to `Hospital.Id`

These relationships are enforced in EF Core and used to ensure data isolation per tenant.

### `Hospital`

- `Id`
- `Name`
- `Slug` (used as the value of the `X-Tenant-Id` header)

### `Patient`

- `Id`
- `Name`
- `DietaryRestrictionCode` (enum)
- `HospitalId` (FK → `Hospital.Id`)

### `MenuItem`

- `Id`
- `Name`
- flags like `IsGlutenFree`, `IsSugarFree`, etc.
- `HospitalId` (FK → `Hospital.Id`)

### `DietaryRestriction` (enum)

Represents a patient’s dietary restriction, used to filter menu items.

---

## API Endpoints (examples)

The controllers follow a REST-ish style.

### Patients

- `GET /api/patients` → list patients
- `GET /api/patients/{id}` → patient by id
- `POST /api/patients` → create patient
- `PUT /api/patients/{id}` → update patient
- `DELETE /api/patients/{id}` → delete patient

#### Allowed menu for a patient

- `GET /api/patients/{patientId}/allowed-menu`

Response:

- `200 OK` with menu items when items exist
- `404 Not Found` when no allowed items are found

### Menu items

- `GET /api/menu-items/{id}` → menu item by id
- `POST /api/menu-item/` → create menu item
- `PUT /api/menu-items/{id}` → update menu item
- `DELETE /api/menu-items/{id}` → delete menu item

Tip: There is a ready-to-use HTTP scratch file at `Computrition.MenuService.API.http`.

---

## Data Access: EF Core vs Dapper (why both)

This project intentionally demonstrates **two approaches**:

- **EF Core** is excellent for CRUD and update flows where you want:
	- change tracking
	- relationship mapping (if expanded later)
	- migrations (if you add them)
	- less handwritten SQL

- **Dapper** is excellent for read-heavy endpoints where you want:
	- very fast execution
	- explicit SQL you can tune
	- predictable queries without EF overhead

In real systems, it’s common to use EF Core for writes and a micro-ORM like Dapper for performance-critical reads.

### Example: EF Core is a great fit for updates

Updating a menu item is safest when EF tracks a single entity instance per primary key.
Recommended approach:

1. Load the existing row (tracked)
2. Copy values
3. Save

This avoids the common EF error:

> The instance of entity type 'MenuItem' cannot be tracked because another instance with the same key value is already being tracked.

Why it happens: you loaded a tracked entity, then tried to attach/update a second instance (often from the request body) with the same key.

### Example: Dapper is a good fit for filtered menu reads

Filtering menu items by dietary rules is a classic read scenario. With Dapper you can write an explicit query like:

```sql
SELECT Id, Name, IsGlutenFree, IsSugarFree, ...
FROM MenuItems
WHERE IsGlutenFree = 1;
```

This is easy to optimize and keeps read queries very transparent.

In this codebase, this is implemented with Dapper in:

- [Computrition.MenuService.API/Repositories/MenuRepository.cs](Computrition.MenuService.API/Repositories/MenuRepository.cs) (`GetFilteredMenuItemsAsync`) using `_dapperConn.QueryAsync<MenuItem>(sql)`
- [Computrition.MenuService.API/Repositories/PatientRepository.cs](Computrition.MenuService.API/Repositories/PatientRepository.cs) (`GetAllAsync`, `GetPatientByIdAsync`) using `_dapperConn.QueryAsync<Patient>(...)` / `_dapperConn.QueryFirstOrDefaultAsync<Patient>(...)`

Concrete example from `MenuRepository.GetFilteredMenuItemsAsync` (dynamic SQL based on the restriction code):

```csharp
string sql = "SELECT * FROM MenuItems WHERE HospitalId = @HospitalId";
if (restriction == DietaryRestriction.GF) sql += " AND IsGlutenFree = 1";
if (restriction == DietaryRestriction.SF) sql += " AND IsSugarFree = 1";
if (restriction == DietaryRestriction.HH) sql += " AND IsHeartHealthy = 1";

return await _dapperConn.QueryAsync<MenuItem>(sql, new { HospitalId = _tenant.HospitalId });
```

**Rule of thumb used here:**

- Use **EF Core** for create/update/delete where tracking is helpful
- Use **Dapper** for read endpoints where you want tight control over SQL

Where EF Core is used in this project:

- [Computrition.MenuService.API/Repositories/MenuRepository.cs](Computrition.MenuService.API/Repositories/MenuRepository.cs) (`AddMenuItemAsync`, `DeleteMenuItem`, `UpdateAsync`) using `_efContext.MenuItems.Add/Remove/Update` + `SaveChangesAsync`
- [Computrition.MenuService.API/Repositories/PatientRepository.cs](Computrition.MenuService.API/Repositories/PatientRepository.cs) (`CreateAsync`, `UpdateAsync`, `DeleteAsync`) using `_efContext.Patients.Add/Update/Remove` + `SaveChangesAsync`

---

## Running Locally

### Prerequisites

- .NET SDK 8.x (`dotnet --version`)

### Build

```bash
dotnet build
```

### Run the API

```bash
dotnet run --project Computrition.MenuService.API
```

Then open Swagger (if enabled) or use the included HTTP file:

- `Computrition.MenuService.API.http`

---

## Swagger (how to send X-Tenant-Id)

Swagger is configured in [Computrition.MenuService.API/Program.cs](Computrition.MenuService.API/Program.cs) with an API-key style header named `X-Tenant-Id`.

How to use it:

1. Run the API.
2. Open Swagger UI.
3. Click **Authorize**.
4. Enter a hospital slug (for seeded tenants: `hospital-a` or `hospital-b`).
5. Execute any tenant-scoped endpoint (patients/menu items). The header will be sent automatically.

### Swagger UI example (Authorize modal)

When you click **Authorize**, Swagger prompts you for the `X-Tenant-Id` value. After you enter a slug (for example `hospital-a`), Swagger will attach the header to every request you execute from the UI.

Save the screenshot below as `docs/swagger-tenant-header.png` to render it in GitHub:

![Swagger UI showing X-Tenant-Id authorization](docs/swagger-tenant-header.png)

Quick verification:

- Set `X-Tenant-Id = hospital-a` and call `GET /api/patients` → all returned rows have `hospitalId = 1`
- Set `X-Tenant-Id = hospital-b` and call `GET /api/patients` → all returned rows have `hospitalId = 2`

Hospital endpoints under `/api/hospital` do not require the header because they are annotated with `[SkipTenantHeader]`.

---

## Tests

This repository includes both **unit tests** (service logic) and **integration-style tests** for multi-tenancy.

### Run tests

```bash
dotnet test
```

### What is tested

- `MenuServiceTests.cs`
	- Allowed menu filtering behavior
	- Validations (e.g., name required)
	- Normalization (capitalizing first letter)

- `PatientServiceTests.cs`
	- Patient CRUD behaviors (via mocked repository)

- `MultiTenancyControllerTests.cs`
	- Missing `X-Tenant-Id` returns `400`
	- Unknown tenant slug returns `401`
	- Tenant A only sees rows where `HospitalId = 1`
	- Tenant B only sees rows where `HospitalId = 2`
	- Tenant A cannot fetch Tenant B’s patient by id (returns `404`)
	- Hospital endpoints are accessible without the tenant header (`[SkipTenantHeader]`)

These tests boot the API in-process using `WebApplicationFactory` and a shared in-memory SQLite database:

- [Computrition.MenuService.Tests/TestInfrastructure/TestWebApplicationFactory.cs](Computrition.MenuService.Tests/TestInfrastructure/TestWebApplicationFactory.cs)

### Why mock repositories?

- Tests focus on business logic, not persistence
- Keeps tests fast and deterministic

---

## Common Pitfalls (EF tracking)

If you see this exception:

> The instance of entity type 'MenuItem' cannot be tracked because another instance with the same key value ...

It usually means:

- You loaded an entity (tracked) using EF
- Then you called `Update()` or `Attach()` using a second instance with the same `Id` (often the incoming request object)

**Fix:** update the tracked instance instead of attaching a new one.

---

## Notes / Improvements

If expanding this into a production-ready microservice, the next steps would be:

- Add DTOs and validation (FluentValidation) instead of using entity types directly in controllers
- Add global exception handling middleware + consistent error responses
- Add migrations and environment-specific DB configuration
- Add observability (structured logging, tracing, metrics)
- Consider DB-level enforcement (Row-Level Security) if moving beyond SQLite
