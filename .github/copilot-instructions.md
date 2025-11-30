# ALOD/ECT System - AI Coding Agent Instructions

## Project Overview
This is the **ALOD (Air Force Line of Duty)** legacy enterprise system—a .NET Framework 4.8.1 web application stack managing military line-of-duty investigations, appeals, and special cases. Built with ASP.NET WebForms (VB.NET/C#), NHibernate ORM, and a layered architecture pattern.

## Architecture

### Layered Structure (Dependency Flow)
```
ALOD (VB.NET WebForms) + SRXLite (VB.NET WebForms)
    ↓
ALODWebUtility (VB.NET shared utilities)
    ↓
ALOD.Core (C# domain models + interfaces)
    ↓
ALOD.Data (C# data access - NHibernate DAOs)
    ↓
ALOD.Logging (C# logging - Enterprise Library)
```

- **Web Applications**: `ALOD/` and `SRXLite/` are ASP.NET WebForms apps (VB.NET) with `Secure/` folders containing protected pages
- **Domain Layer**: `ALOD.Core/Domain/` contains business entities (inheriting from `BaseObject`), organized into modules like `Modules/Lod/`, `Modules/Appeals/`, `Documents/`, `Users/`, etc.
- **Data Access**: `ALOD.Data/` implements NHibernate DAOs following the Repository pattern. Each DAO implements an interface from `ALOD.Core/Interfaces/DAOInterfaces/`
- **Dependency Injection**: Uses `NHibernateDaoFactory` (implements `IDaoFactory`) to instantiate DAOs—**not** a modern DI container
- **ORM Mappings**: NHibernate XML mappings in `ALOD.Core/Mappings/*.hbm.xml` (e.g., `<class name="LineOfDuty" table="Form348">`)

### Key Patterns
- **DAO Pattern**: Every entity has a DAO interface (e.g., `ILineOfDutyDao`) and implementation (e.g., `LineOfDutyDao : AbstractNHibernateDao`)
- **Session Management**: `NHibernateSessionManager` (singleton) manages per-request sessions via `HttpContext.Current.Items["CONTEXT_SESSION"]`
- **Domain Signature**: Domain objects use `BaseObject` which implements `Equals()`/`GetHashCode()` based on properties marked with `[DomainSignature]`
- **Configuration Externalization**: Connection strings and app settings in `configFiles/*.config` (referenced via `configSource` attribute)

## Build System

### VS Code Build Commands (Ctrl+Shift+B)
- **Default Build**: `dotnet build AFLOD.sln` (Debug configuration)
- **Release Build**: Task "build solution (Release)"
- **Rebuild**: Task "rebuild solution" (clean + build)
- **Individual Projects**: Tasks exist for `ALOD.Core`, `ALOD.Data`, etc.

### Build Configuration (Simplified November 2025)
- **Solution Configurations**: Only **Debug|Any CPU** and **Release|Any CPU** (10 legacy configurations removed)
- **All Projects**: Build as AnyCPU for maximum portability
- **ALOD.Tests** is **included** in solution with xUnit 2.4.2 testing framework
- **No Platform-Specific Configs**: All x86/x64/Win32/Mixed Platforms configurations removed from solution and project files

### Deployment Scripts
- `BuildScripts/*.ps1` — PowerShell post-build scripts for environment-specific deployments (AFNET-Prod, SSI-Test, etc.)
- Deploys to IIS with web.config transformations via `Web.Debug.config` / `Web.Release.config`

### IIS Deployment Best Practice
- **CRITICAL**: When deploying to IIS (`C:\inetpub\ALOD\` or `C:\inetpub\SRXLite\`), **always delete all files in the target directory first** before copying new files
- This prevents assembly version conflicts caused by outdated DLLs remaining in the deployment folder
- Workflow: `Clean Target → Copy New Files → Transform web.config → Restart IIS`
- Never do partial deployments by copying individual DLLs—this leads to version mismatches and TypeLoadExceptions

## Critical Dependencies

### PDF Generation
- **ABCpdf 12.5.1** — Used extensively in `ALOD.Core/` and web utilities. License file must exist in `Referenced Assemblies/ABCpdf.DLL`
- Forms are rendered to PDF using `Doc.AddImageUrl()` and custom PDF templates

### Database/ORM
- **NHibernate 5.x** with SQL Server 2005+ dialect
- **Connection String**: Named "LOD" in `connectionStrings.config` (points to MSSQL database)
- **Schema**: `ALOD.dbo` — Tables follow Form348_* naming (e.g., `Form348`, `Form348_AP_SARC`, `Form348_SC`)

### Key NuGet Packages
- `AutoMapper 10.0` — Entity-to-DTO mapping (check `ALOD.Core/Mappings/` for AutoMapper profiles)
- `MailKit 4.2` / `MimeKit 4.2` — Email functionality
- `NPOI 2.6.2` — Excel export
- `log4net 2.0.15` + Enterprise Library — Logging infrastructure

## Development Patterns

### Adding a New DAO
1. Define interface in `ALOD.Core/Interfaces/DAOInterfaces/INewEntityDao.cs`
2. Implement in `ALOD.Data/NewEntityDao.cs : AbstractNHibernateDao<TEntity>`
3. Add factory method to `IDaoFactory` interface and `NHibernateDaoFactory` class
4. Create NHibernate mapping in `ALOD.Core/Mappings/NewEntity.hbm.xml`

### Session Management Pattern
```csharp
// DAOs use: NHibernateSessionManager.Instance.GetSession()
ISession session = NHibernateSessionManager.Instance.GetSession();
```
- Sessions are per-HTTP-request (closed by `NHibernateSessionModule`)
- **Never** manually dispose sessions—handled by framework

### Configuration Access
```vb
' VB.NET WebForms code
Dim connString = ConfigurationManager.ConnectionStrings("LOD").ConnectionString
Dim setting = ConfigurationManager.AppSettings("SomeKey")
```
- Sensitive settings use **User Secrets** (see `web.config` `configBuilders` section with ID `39febcb4-89b9-48a5-911a-3c404a9734c1`)

### PowerShell Commands
- **MANDATORY**: Always use the PowerShell Extension terminal for PowerShell commands
- **NEVER** use `Start-Process powershell -Verb RunAs` or external PowerShell processes
- Use `run_in_terminal` tool with the PowerShell Extension terminal for all PowerShell operations

## Testing Guidance

### Unit Tests (VS Code Compatible)
- `ALOD.Tests/` — xUnit 2.4.2 project with Moq for mocking
- Tests can be run in VS Code using `dotnet test` command

### Running Tests
```bash
# Run tests in VS Code
dotnet test ALOD.Tests/ALOD.Tests.csproj --configuration Debug --verbosity normal
```

### Creating New Unit Tests - Best Practices
- **Always verify domain entity properties first** — Read the actual entity source files in `ALOD.Core/Domain/` before writing tests. Never assume property names.
- **Use correct return types** — DAO methods like `GetAll()` return `IQueryable<T>`, not `List<T>`. Include `.AsQueryable()` on test data collections.
- **Include System.Linq namespace** — Required for `.AsQueryable()` extension method on test collections.
- **Keep tests simple** — Verify non-null results and mock invocations rather than asserting on specific property values. This keeps tests maintainable as the domain evolves.
- **Read before generating** — For multiple test files, read all relevant domain entities in parallel first, then create correct tests from the start to avoid compile-test-fix cycles.
- **Check for namespace conflicts** — Before creating tests, verify that entity names don't conflict with xUnit or other framework namespaces (e.g., `Unit` conflicts with xUnit). Use type aliases when needed: `using DomainUnit = ALOD.Core.Domain.Users.Unit;`
- **Avoid duplicate test files** — Always check if a test file already exists before creating it. Use `file_search` to verify the test doesn't already exist in `ALOD.Tests/Unit/Data/`.

## Common Issues

### "Could not load file or assembly 'ABCpdf'"
- Ensure `Referenced Assemblies/ABCpdf.DLL` exists and NuGet package is restored
- Check `web.config` for assembly binding redirects

### NHibernate Session Errors
- Verify connection string in `configFiles/connectionStrings.config`
- Check `hibernate-configuration` section in `web.config` (property `connection.connection_string_name` = "LOD")
- Ensure database schema matches mappings (`default_schema` = "ALOD.dbo")

### Build Configuration Warnings
- If seeing "Debug symbols not found" or "PDB mismatch", run `dotnet clean` then `dotnet build`
- Platform target mismatches (AnyCPU vs x64) are documented in `Build_Configuration_Report.md`—generally safe to ignore

### Web Application Not Running
- **IIS Express Required** — ASP.NET WebForms apps need IIS Express or full IIS (not Kestrel)
- Configure IIS Express via `.vs/config/applicationhost.config` (created by Visual Studio 2022)

## File Naming Conventions

- **Domain Entities**: `ALOD.Core/Domain/<Module>/<Entity>.cs` (e.g., `Modules/Lod/LineOfDuty.cs`)
- **DAOs**: `ALOD.Data/<Entity>Dao.cs` (e.g., `LineOfDutyDao.cs`)
- **Interfaces**: `ALOD.Core/Interfaces/DAOInterfaces/I<Entity>Dao.cs`
- **Mappings**: `ALOD.Core/Mappings/<Entity>.hbm.xml`
- **WebForms Pages**: `ALOD/Secure/<Module>/<Page>.aspx` + `.aspx.vb` + `.aspx.designer.vb`

## Migration Notes

- **VS Code Compatible** — Solution builds via `dotnet CLI` as of November 2025 (see `VS_Code_Migration_Summary.md`)
- **Original Platform**: Visual Studio 2010-2022, migrated from .NET Framework 4.0 → 4.8.1
- **Modernization Opportunity**: Tests already use xUnit 2.4.2; consider upgrading to xUnit 3.x, but **do not** migrate web apps to ASP.NET Core (requires full rewrite)

## Additional Resources

- `Documentation/Build_Configuration_Report.md` — Comprehensive build configuration analysis (15 issues documented, framework upgrades completed November 2025)
- `LOD_Field_Update_Analysis.md` — Database field mapping reference
- `Referenced Assemblies/` — Third-party DLLs not available via NuGet (ABCpdf, ReportViewer)
