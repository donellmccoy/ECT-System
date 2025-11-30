# Connection String Usage Analysis

## Overview
This document provides a comprehensive analysis of all code in the ALOD/ECT System that uses the connection strings defined in `ALOD\configFiles\connectionStrings.config`.

## Connection Strings Defined

### 1. "LOD" Connection String
**Current Active Configuration:**
```xml
<add name="LOD" connectionString="Server=localhost;Database=ALOD_Test;Integrated Security=True;" providerName="System.Data.SqlClient"/>
```

**Alternate Configurations (Commented):**
- Remote Azure SQL: `Server=104.45.151.80;Initial Catalog=ALOD`
- Remote Azure SQL: `Server=20.81.191.170;Initial Catalog=ALOD`
- Local SQL Auth: `Server=localhost;Initial Catalog=ALOD`

### 2. "SRXLiteConnectionString" Connection String
**Current Active Configuration:**
```xml
<add name="SRXLiteConnectionString" connectionString="Server=localhost;Initial Catalog=SRXLite;User ID=testuser;Password=Password123!@#;" providerName="System.Data.SqlClient"/>
```

**Alternate Configurations (Commented):**
- Remote Azure SQL: `Server=20.81.191.170;Initial Catalog=SRXLite`

---

## "LOD" Connection String Usage

This is the primary connection string used throughout the ALOD system. It connects to the main ALOD database.

### NHibernate ORM Configuration

The LOD connection string is configured as the primary data access mechanism through NHibernate.

#### Configuration Files:
- **`ALOD\web.config`** (Line 31)
  ```xml
  <property name="connection.connection_string_name">LOD</property>
  ```
  - Configures NHibernate to use the "LOD" connection string
  - Used by entire ALOD.Data layer for ORM operations

- **`ALOD.Tests\App.config`** (Line 17)
  - Test project configuration using same connection string
  
- **`Snapshot\App.config`** (Line 31)
  - Snapshot utility configuration

#### Code Implementation:
- **`ALOD.Data\NHibernateSessionManager.cs`**
  - Manages NHibernate sessions using the LOD connection
  - Provides session-per-request pattern via `HttpContext.Current.Items["CONTEXT_SESSION"]`
  - All DAOs in `ALOD.Data\` namespace use this connection through the session manager

### Direct ADO.NET Usage

Several pages bypass NHibernate and use direct ADO.NET connections.

#### User Interface Components:
- **`ALOD\Secure\Shared\UserControls\CaseDialogue.ascx.vb`** (Lines 351, 399)
  ```vb
  Dim conn As New SqlConnection(ConfigurationManager.ConnectionStrings("LOD").ConnectionString)
  ```
  - Used in `GenerateLODPDFDataConvert()` method
  - Creates direct connections for PDF generation operations

#### Administrative Pages:
- **`ALOD\Secure\Shared\Admin\Snapshot.aspx.vb`** (Line 314)
  ```vb
  Dim conn As New SqlConnection(ConfigurationManager.ConnectionStrings("LOD").ConnectionString)
  ```
  - Used in snapshot/backup operations
  - Direct database operations for data export

#### DataSet Designer:
- **`ALOD\LODDataSet.Designer.vb`** (Line 1588)
  ```vb
  Me._connection.ConnectionString = Global.System.Configuration.ConfigurationManager.ConnectionStrings("LOD").ConnectionString
  ```
  - Typed dataset using LOD connection
  - Generated code for data binding operations

#### Utility Applications:
- **`Snapshot\Program.cs`** (Line 55)
  ```csharp
  using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["LOD"].ConnectionString))
  ```
  - Standalone snapshot utility
  - Direct database access for backup operations

### LINQ to SQL DataContext (Legacy)

Legacy code in the `Old_App_Code` directory uses LINQ to SQL with the LOD connection.

- **`ALOD\Old_App_Code\AFLOD.designer.vb`** (Line 157)
  ```vb
  Public Sub New()
      MyBase.New(Global.System.Configuration.ConfigurationManager.ConnectionStrings("LOD").ConnectionString, mappingSource)
  ```
  
- **`ALOD\Old_App_Code\AFLOD1.designer.vb`** (Line 157)
  - Similar LINQ to SQL context
  
- **`ALOD\Old_App_Code\Admin.designer.vb.exclude`** (Line 61)
  - Excluded legacy code (not currently compiled)

**Note:** These are legacy files maintained for reference. Current development uses NHibernate ORM.

### Session State Configuration

- **`SubWebConfigs\SSI-Test\ALOD\sessionState.config`**
  ```xml
  <sessionState mode="SQLServer" 
                sqlConnectionString="LOD" 
                cookieless="false" 
                timeout="20" />
  ```
  - SQL Server-based session state storage
  - Uses LOD connection for session persistence

---

## "SRXLiteConnectionString" Connection String Usage

This connection string is used exclusively by the SRXLite module, which operates as a separate application with its own database.

### SRXLite Data Access Layer

The SRXLite module has its own data access utilities separate from the main ALOD NHibernate layer.

#### Primary Data Access:
- **`SRXLiteWebUtility\DataAccess\DB.vb`** (Line 21)
  ```vb
  Public Shared Function getConnectionString() As String
      Return ConfigurationManager.ConnectionStrings("SRXLiteConnectionString").ConnectionString
  End Function
  ```
  - Central connection string retrieval method
  - Used by all SRXLite data access operations

#### Asynchronous Operations:
- **`SRXLiteWebUtility\DataAccess\AsyncDB.vb`** (Lines 27, 37)
  ```vb
  Dim connectionString As String = ConfigurationManager.ConnectionStrings("SRXLiteConnectionString").ConnectionString
  ```
  - Used for asynchronous database operations
  - Separate async data access pattern for SRXLite

### Snapshot Utility (Commented Out)

The snapshot utility has commented-out code for SRXLite database operations:

- **`Snapshot\Program.cs`** (Line 171 - commented)
  ```csharp
  //using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SRXLiteConnectionString"].ConnectionString))
  ```
  - Currently disabled
  - Would handle SRXLite database snapshots

- **`ALOD\Secure\Shared\Admin\Snapshot.aspx.vb`** (Line 368 - commented)
  - Similar commented-out SRXLite snapshot functionality

---

## Architecture Pattern Summary

### Primary Data Access (ALOD)
```
Web Layer (VB.NET WebForms)
    ↓
NHibernate Session Manager (uses "LOD")
    ↓
ALOD.Data DAOs (AbstractNHibernateDao)
    ↓
SQL Server Database (ALOD_Test / ALOD)
```

### Secondary Data Access (SRXLite)
```
SRXLite Web Layer (VB.NET WebForms)
    ↓
SRXLiteWebUtility.DB / AsyncDB (uses "SRXLiteConnectionString")
    ↓
Direct ADO.NET SqlConnection
    ↓
SQL Server Database (SRXLite)
```

### Direct ADO.NET Usage (Special Cases)
- PDF generation operations
- Administrative snapshot/backup operations
- Legacy DataSet operations
- Session state persistence

---

## Configuration Management

### Connection String Resolution
1. **External Configuration File**: `ALOD\configFiles\connectionStrings.config`
2. **Referenced via**: `web.config` using `configSource` attribute
3. **Accessed via**: `ConfigurationManager.ConnectionStrings["name"]`
4. **NHibernate Access**: Via `connection.connection_string_name` property

### Environment-Specific Configurations
The system supports multiple environments through commented connection strings:
- **Local Development**: Windows Authentication (currently active)
- **Local Development**: SQL Authentication (for JDBC/cross-platform)
- **Remote Azure**: SQL Authentication with specific endpoints

### Build Process
Post-build scripts in `BuildScripts\*.ps1` handle environment-specific deployments with web.config transformations:
- `Web.Debug.config`
- `Web.Release.config`

---

## Security Considerations

### Current Configuration
- **LOD Connection**: Windows Authentication (Integrated Security)
  - No credentials in connection string
  - Uses Windows security context
  - Recommended for local development

- **SRXLiteConnectionString**: SQL Server Authentication
  - Credentials embedded in connection string
  - Required for JDBC connectivity
  - Should use User Secrets or Azure Key Vault in production

### User Secrets
The `web.config` references User Secrets via `configBuilders` section:
```xml
<configBuilders>
    <builders>
        <add name="Secrets" userSecretsId="39febcb4-89b9-48a5-911a-3c404a9734c1" ... />
    </builders>
</configBuilders>
```

### Recommendations
1. Never commit actual production credentials to source control
2. Use Windows Authentication where possible
3. For production SQL Auth, use Azure Key Vault or User Secrets
4. Rotate credentials regularly
5. Use least-privilege database accounts

---

## Migration Notes

### From Legacy to Current
- **Legacy**: LINQ to SQL DataContext (in `Old_App_Code\`)
- **Current**: NHibernate ORM (in `ALOD.Data\`)
- **Migration Status**: Legacy code retained for reference, not compiled

### Future Considerations
- Consider migrating direct ADO.NET usage to NHibernate for consistency
- Evaluate consolidating SRXLite data access to use NHibernate pattern
- Review session state storage strategy (SQL vs Redis vs InProc)

---

## Troubleshooting

### Common Connection Issues

#### "Could not open connection to SQL Server"
- Verify SQL Server is running: `Get-Service MSSQL*`
- Check connection string server name
- For Windows Auth, verify account has database access

#### "Login failed for user"
- For SQL Auth, verify credentials in connection string
- Check SQL Server authentication mode (Windows + SQL)
- Verify database user has appropriate permissions

#### NHibernate Session Errors
- Ensure `hibernate-configuration` section exists in `web.config`
- Verify `connection.connection_string_name` matches connection string name
- Check `default_schema` matches database schema (default: "ALOD.dbo")

#### Database Not Found
- For LOD: Current config uses database name "ALOD_Test"
- For SRXLite: Uses database name "SRXLite"
- Verify databases exist on SQL Server instance

---

## Testing Connection Strings

### Quick Verification
```powershell
# Test SQL Server connectivity
Test-NetConnection -ComputerName localhost -Port 1433

# List available databases
sqlcmd -S localhost -Q "SELECT name FROM sys.databases" -E
```

### Code-Level Testing
The `ALOD.Tests` project includes connection tests:
- Test connection string configuration
- Verify NHibernate session creation
- Validate database connectivity

---

## Related Documentation

- `Documentation/Build_Configuration_Report.md` - Build configuration analysis
- `LOD_Field_Update_Analysis.md` - Database field mapping
- `.github/copilot-instructions.md` - Architecture overview
- `VS_Code_Migration_Summary.md` - VS Code migration notes

---

**Document Generated**: October 21, 2025  
**Analysis Scope**: All connection string usage in AFLOD.sln  
**Connection Strings Analyzed**: LOD, SRXLiteConnectionString
