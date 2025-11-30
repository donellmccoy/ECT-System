# ALOD System - AI Coding Assistant Instructions



## System Overview
The ALOD (Air Force Line of Duty) system is a military/government web application for managing Line of Duty investigations, appeals, and special case processing. It handles complex workflows for determining service member eligibility for benefits related to injuries or illnesses incurred during military service.

## Architecture & Technology Stack

### Core Technologies

- **Frontend**: ASP.NET Web Forms (VB.NET) with AjaxControlToolkit
- **Backend**: .NET Framework 4.8, C# class libraries
- **Database**: SQL Server with NHibernate ORM
- **PDF Generation**: ABCpdf library
- **Reporting**: Microsoft ReportViewer
- **Logging**: Microsoft Enterprise Library + custom LogManager
- **Dependency Injection**: Microsoft Unity
- **Testing**: xUnit 2.4.2 framework

### Project Structure

```text
AFLOD.sln (main solution)
├── ALOD/ (main web app - VB.NET Web Forms)
├── ALOD.Core/ (domain models, business logic - C#)
├── ALOD.Data/ (data access layer - C#)
├── ALOD.Logging/ (logging infrastructure - C#)
├── ALOD.Tests/ (unit tests - C#)
└── ALODWebUtility/ (utility library - VB.NET)
```

### Domain-Driven Design

- **Domain Layer** (`ALOD.Core/Domain/`): Rich domain models with business logic
- **Entity Base Class**: All domain entities inherit from `Entity` with `Id` property
- **NHibernate Mappings**: XML mapping files (`.hbm.xml`) in `ALOD.Core/Mappings/`

### Workflow System

- **Complex State Machines**: 40+ workflow types (LOD, Appeals, Special Cases, SARC)
- **Status-Based Processing**: Each module has specific status codes and transitions
- **Rule-Based Actions**: Workflow rules determine available actions and validations
- **Key Enums**: `AFRCWorkflows`, `ModuleType`, `OptionRules`, `WorkflowActionType`

### Data Access Patterns

- **Repository Pattern**: DAO interfaces in `ALOD.Core/Interfaces/DAOInterfaces/`
- **NHibernate Sessions**: Managed through `NHibernateSessionManager`
- **Enterprise Library**: Database access via `DatabaseFactory.CreateDatabase()`
- **Stored Procedures**: Extensive use of SQL stored procedures for complex operations

## Critical Development Workflows

### Build Process

- **TFS Integration**: Uses Team Foundation Server with custom PowerShell build scripts
- **Environment Configs**: Different `web.config` files for each environment (SSI-Test, AFNET-Test, AFNET-Prod)
- **Config Builders**: Uses `Microsoft.Configuration.ConfigurationBuilders.UserSecrets` for secrets management
- **Build Scripts**: Located in `BuildScripts/` - run pre-build config file management

### Testing Approach

- **NHibernate Integration Tests**: Tests extend `NHibernateTestCase` for database integration
- **Transaction Rollback**: All tests use transaction rollback to maintain test isolation
- **Session Management**: Tests initialize NHibernate sessions in `CallContext`

### Configuration Management

- **External Config Files**: Connection strings, app settings, mail settings in `configFiles/` directory
- **NHibernate Config**: Database schema `ALOD.dbo`, SQL Server dialect
- **Multiple Environments**: Config files swapped during build process

## Project-Specific Conventions

### Naming Conventions

- **Database Schema**: `ALOD.dbo` prefix for all tables
- **Stored Procedures**: `core_*` prefix for core operations, module-specific prefixes
- **Enums**: PascalCase with descriptive names (e.g., `AFRCWorkflows.LOD`)
- **Domain Classes**: Follow NHibernate mapping file names

### Logging Patterns

- **Centralized Logging**: All logging through `ALOD.Logging.LogManager`
- **Context-Aware**: Automatically captures user ID, IP, session data
- **Action Logging**: `LogAction()` method for user action tracking
- **Error Logging**: Comprehensive error logging with stack traces

### Security & Permissions

- **Role-Based Access**: Complex permission system with user groups and roles
- **Page-Level Security**: `PageAccess` controls read/write permissions per workflow status
- **Approval Authorities**: Hierarchical approval chains for different case types

### Document Processing

- **PDF Generation**: ABCpdf for dynamic PDF creation from templates
- **Digital Signatures**: Integrated signature workflow with template management
- **Document Storage**: File system storage with metadata tracking

## Integration Points

### External Dependencies

- **ABCpdf**: PDF generation and manipulation
- **ReportViewer**: RDLC report generation
- **Azure Storage**: Blob storage for file uploads (Azure.Storage.Blobs)
- **MailKit**: Email sending capabilities
- **Entity Framework**: Limited use alongside NHibernate

### Cross-Component Communication

- **Module Boundaries**: Clear separation between LOD, Appeals, Special Cases, SARC modules
- **Shared Services**: Common utilities in `ALODWebUtility` and `ALOD.Core`
- **Database Integration**: All components share the same SQL Server database
- **Session State**: Shared session state across web application components

## Common Development Tasks

### Adding New Workflow Status

1. Add status code to relevant enum in `WorkflowEnums.cs`
2. Update workflow rules in database
3. Add UI handling in ASPX pages
4. Update permission mappings

### Database Schema Changes

1. Modify NHibernate mapping files (`.hbm.xml`)
2. Update database scripts in `ALOD.Database/Scripts/`
3. Regenerate entity classes if needed
4. Update DAO interfaces and implementations

### Adding New Reports

1. Create RDLC report file in `ALOD/Reports/`
2. Add report args class in `ALOD.Core/Domain/Reports/`
3. Implement report DAO method
4. Add UI integration in web forms

## Code Quality Standards

### StyleCop Compliance

- **Enabled Rules**: All rules in `AllRules.ruleset`
- **Analysis Level**: Warnings treated as errors in CI
- **File Headers**: Standard Microsoft headers required

### Error Handling

- **Try-Catch Blocks**: Comprehensive exception handling
- **Logging**: All exceptions logged with full context
- **User-Friendly Messages**: Generic error messages for users, detailed logging for developers

### Performance Considerations

- **NHibernate Optimization**: Careful use of lazy loading and session management
- **Database Queries**: Prefer stored procedures for complex operations
- **Memory Management**: Use of recyclable memory streams for large file operations

## Testing Guidelines

### Unit Test Structure

- **Base Class**: Extend `NHibernateTestCase` for database tests
- **Setup/Teardown**: Proper session initialization and transaction rollback
- **Test Data**: Use factory methods or test data builders
- **Assertion Style**: Clear, descriptive assertions with meaningful messages

### Integration Testing

- **Database State**: Tests should not depend on specific database state
- **Isolation**: Each test should be completely isolated
- **Cleanup**: Automatic cleanup through transaction rollback

## Deployment Considerations

### Environment Differences

- **Config Files**: Different connection strings and settings per environment
- **Permissions**: Database permissions vary by environment
- **External Services**: Different service endpoints for test vs production

### Build Artifacts

- **Web Application**: Main deployable unit
- **Database Scripts**: Incremental schema updates
- **Config Files**: Environment-specific configuration
- **Dependencies**: NuGet packages and external assemblies

## Troubleshooting Common Issues

### NHibernate Session Issues

- **Session Management**: Ensure proper session lifecycle in web requests
- **Transaction Scope**: Use transaction scopes for multi-operation tasks
- **Lazy Loading**: Be aware of N+1 query problems

### Configuration Problems

- **Config File Paths**: Ensure correct relative paths to config files
- **Connection Strings**: Verify database connectivity and permissions
- **Assembly References**: Check version compatibility of external libraries

### Workflow State Issues

- **Status Transitions**: Validate all required fields before status changes
- **Permission Checks**: Ensure user has appropriate permissions for actions
- **Business Rules**: Complex validation rules may block status transitions

This system represents a mature, complex enterprise application with deep domain knowledge requirements. Always reference the existing codebase patterns and consult domain experts for business rule changes.
