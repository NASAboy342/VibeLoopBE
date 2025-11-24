<!--
  ============================================================================
  SYNC IMPACT REPORT
  ============================================================================
  Version Change: TEMPLATE → 1.0.0 (MAJOR - Initial constitution ratification)
  
  Modified Principles:
  - NEW: I. .NET 8 & Dapper ORM (Data access architecture)
  - NEW: II. SQLite Local Database (Database choice and management)
  - NEW: III. MVC Folder Structure (Code organization)
  - NEW: IV. Naming Conventions (File and class naming standards)
  - NEW: V. CORS Policy (Security and cross-origin access)
  
  Added Sections:
  - Technology Stack (Section 2)
  - Code Organization Standards (Section 3)
  - Governance rules established
  
  Removed Sections:
  - None (initial creation)
  
  Templates Requiring Updates:
  - ✅ plan-template.md (verified - Constitution Check section is flexible)
  - ✅ spec-template.md (verified - user stories are architecture-agnostic)
  - ✅ tasks-template.md (verified - task structure adapts to project type)
  - ✅ checklist-template.md (verified - template is generic)
  
  Follow-up TODOs:
  - None - All placeholders filled
  
  Notes:
  - First constitution version for VibeLoopBE project
  - Ratification date set to today (2025-11-24)
  - All templates reviewed and confirmed compatible with .NET/Dapper/SQLite stack
  ============================================================================
-->

# VibeLoopBE Constitution

## Core Principles

### I. .NET 8 & Dapper ORM

**MUST** use .NET 8 Web API framework for all backend development; **MUST** use Dapper for all data access operations; **MUST NOT** use Entity Framework or any ORM other than Dapper.

**Rationale**: Dapper provides lightweight, high-performance data access with direct SQL control, avoiding the complexity and overhead of Entity Framework while maintaining testability and maintainability.

### II. SQLite Local Database

**MUST** use SQLite as the database engine with local file storage; Database file **MUST** be stored locally and version-controlled schema migrations **MUST** be implemented for reproducibility.

**Rationale**: SQLite provides zero-configuration, embedded database capabilities suitable for local development and deployment scenarios, with excellent .NET support and portability.

### III. MVC Folder Structure

**MUST** organize code following MVC architectural pattern with these top-level folders:
- `Controllers/` - API endpoints and request routing
- `Services/` - Business logic layer
- `Models/` - Data models (DTOs, requests, responses, DBOs)
- `Repositories/` - Data access layer (Dapper implementations)
- `Helpers/` - Utility and helper functions
- `Filters/` - Action filters, exception filters, authorization filters

**Rationale**: Clear separation of concerns enables maintainability, testability, and allows multiple developers to work on different layers without conflicts.

### IV. Naming Conventions

**MUST** follow these naming standards:

Request/Response Models:
- Separate by folder: `Models/Requests/` and `Models/Responses/`
- Pattern: `[Action]Request.cs`, `[Action]Response.cs`
- Example: `CreateUserRequest.cs`, `GetUserResponse.cs`

Database Models:
- Folder: `Models/DBOs/`
- Pattern: `[Object]DBO.cs`
- Example: `UserDBO.cs`, `ProductDBO.cs`

**Rationale**: Consistent naming eliminates ambiguity, improves code discoverability, and signals the purpose of each class at a glance.

### V. CORS Policy

**MUST** configure CORS to allow all domains including localhost for development; Production deployment **SHOULD** restrict to specific trusted origins.

**Rationale**: Enables frictionless local development and testing across different ports/domains while preserving the ability to lock down production environments.

## Technology Stack

**Framework**: .NET 8 Web API  
**ORM**: Dapper (NON-NEGOTIABLE)  
**Database**: SQLite (local file-based)  
**API Documentation**: Swagger/OpenAPI (via Swashbuckle)  
**Dependency Injection**: Built-in .NET DI container  

**Architecture Pattern**: MVC with Repository pattern for data access  
**Testing Framework**: xUnit (recommended) or NUnit  

## Code Organization Standards

**File Placement**:
- Controllers **MUST** inherit from `ControllerBase` and use `[ApiController]` attribute
- Services **MUST** be interface-based for dependency injection
- Repositories **MUST** abstract Dapper calls behind interfaces
- All DBOs **MUST** map directly to database tables/views

**Dependency Flow**: Controllers → Services → Repositories → Database  
- Controllers **MUST NOT** directly call Repositories
- Services **MUST NOT** contain data access code (use Repositories)

**Configuration**:
- Database connection string **MUST** be in `appsettings.json`
- Environment-specific overrides **MUST** use `appsettings.Development.json`

## Governance

This constitution supersedes all other development practices and standards. All feature implementations, code reviews, and architectural decisions **MUST** comply with these principles.

**Amendment Process**:
1. Proposed changes **MUST** be documented with rationale
2. Version number **MUST** be incremented following semantic versioning
3. All affected templates and documentation **MUST** be updated
4. Migration plan **MUST** be provided for breaking changes

**Compliance Verification**:
- All pull requests **MUST** verify adherence to naming conventions
- Code reviews **MUST** check for proper folder structure
- Database access **MUST** exclusively use Dapper
- CORS configuration **MUST** be reviewed before production deployment

**Version**: 1.0.0 | **Ratified**: 2025-11-24 | **Last Amended**: 2025-11-24
