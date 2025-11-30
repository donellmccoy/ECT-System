# ALOD Session State Error Fix

## Error Description
When running the ALOD application, the following server error occurs:

```
Server Error in '/' Application.
Cannot insert the value NULL into column 'Flags', table 'ALOD.dbo.ASPStateTempSessions'; column does not allow nulls. INSERT fails.
The statement has been terminated.
```

This is typically caused by a mismatch between the ASP.NET version (e.g., 4.8) and the session state schema in the SQL database (ALOD). The `Flags` column (added in ASP.NET 2.0) is non-nullable without a default value, or the stored procedures are outdated/missing.

Stack trace points to `System.Web.SessionState.SqlSessionStateStore` failing during session insertion.

## Root Causes
- Outdated or incomplete session state schema installation in the ALOD database.
- Schema installed with an older ASP.NET version (e.g., 1.x) that doesn't handle the `Flags` column.
- Permissions issues preventing full schema removal/installation via `aspnet_regsql.exe`.
- Custom database mode (`-sstype c`) requires specific cleanup in the target database and msdb (for SQL Agent jobs).

## Prerequisites
- SQL Server Management Studio (SSMS) for manual verification and cleanup.
- Access to `aspnet_regsql.exe` (usually at `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\aspnet_regsql.exe` for 64-bit).
- Sysadmin access to SQL Server (use Windows Authentication if possible; otherwise, grant temporarily to your SQL user like 'testuser').
- Backup the ALOD database before making changes.

## Resolution Steps

### Step 1: Remove Existing Schema (Using aspnet_regsql)
Run in Command Prompt (admin mode):
```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\aspnet_regsql.exe -S localhost -U testuser -P Password123!@# -d ALOD -ssremove -sstype c
```
- Confirm with 'y' when prompted.
- If it fails on job deletion due to permissions (e.g., SELECT denied on msdb.dbo.sysjobs), proceed to manual cleanup.

### Step 2: Manual Cleanup in SSMS
It is recommended to connect to `localhost` in SSMS using Windows Authentication mode to ensure sysadmin access and avoid permissions issues.

#### Clean Up Tables, Procedures, and Types in ALOD Database
Run against ALOD:
```sql
-- Drop tables
DROP TABLE IF EXISTS dbo.ASPStateTempSessions;
DROP TABLE IF EXISTS dbo.ASPStateTempApplications;

-- Drop procedures
DROP PROCEDURE IF EXISTS dbo.GetMajorVersion;
DROP PROCEDURE IF EXISTS dbo.CreateTempTables;
DROP PROCEDURE IF EXISTS dbo.TempGetVersion;
DROP PROCEDURE IF EXISTS dbo.GetHashCode;
DROP PROCEDURE IF EXISTS dbo.TempGetAppID;
DROP PROCEDURE IF EXISTS dbo.TempGetStateItem;
DROP PROCEDURE IF EXISTS dbo.TempGetStateItem2;
DROP PROCEDURE IF EXISTS dbo.TempGetStateItem3;
DROP PROCEDURE IF EXISTS dbo.TempGetStateItemExclusive;
DROP PROCEDURE IF EXISTS dbo.TempGetStateItemExclusive2;
DROP PROCEDURE IF EXISTS dbo.TempGetStateItemExclusive3;
DROP PROCEDURE IF EXISTS dbo.TempReleaseStateItemExclusive;
DROP PROCEDURE IF EXISTS dbo.TempInsertUninitializedItem;
DROP PROCEDURE IF EXISTS dbo.TempInsertStateItemShort;
DROP PROCEDURE IF EXISTS dbo.TempInsertStateItemLong;
DROP PROCEDURE IF EXISTS dbo.TempUpdateStateItemShort;
DROP PROCEDURE IF EXISTS dbo.TempUpdateStateItemShortNullLong;
DROP PROCEDURE IF EXISTS dbo.TempUpdateStateItemLong;
DROP PROCEDURE IF EXISTS dbo.TempUpdateStateItemLongNullShort;
DROP PROCEDURE IF EXISTS dbo.TempRemoveStateItem;
DROP PROCEDURE IF EXISTS dbo.TempResetTimeout;
DROP PROCEDURE IF EXISTS dbo.DeleteExpiredSessions;

-- Drop obsolete procedures
DROP PROCEDURE IF EXISTS dbo.DropTempTables;
DROP PROCEDURE IF EXISTS dbo.ResetData;

-- Drop types
EXEC sp_droptype 'tSessionId';
EXEC sp_droptype 'tAppName';
EXEC sp_droptype 'tSessionItemShort';
EXEC sp_droptype 'tSessionItemLong';
EXEC sp_droptype 'tTextPtr';
```

#### Delete SQL Agent Job
In SSMS: SQL Server Agent > Jobs > Right-click `ALOD_Job_DeleteExpiredSessions` > Delete.

Or run against msdb:
```sql
USE msdb;
DECLARE @jobname nvarchar(200) = N'ALOD_Job_DeleteExpiredSessions';
EXEC sp_delete_job @job_name = @jobname;
```

If permissions are insufficient, grant them temporarily by running the following scripts:

#### Grant db_owner in all user databases
```sql
DECLARE @dbName NVARCHAR(128)
DECLARE @sql NVARCHAR(MAX)

DECLARE db_cursor CURSOR FOR
SELECT name FROM sys.databases
WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb') -- exclude system DBs
AND state_desc = 'ONLINE'

OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @dbName

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql = '
    USE [' + @dbName + '];

    IF NOT EXISTS (
        SELECT 1 FROM sys.database_principals WHERE name = ''testuser''
    )
    BEGIN
        CREATE USER [testuser] FOR LOGIN [testuser];
    END

    EXEC sp_addrolemember N''db_owner'', N''testuser'';
    '

    EXEC sp_executesql @sql

    FETCH NEXT FROM db_cursor INTO @dbName
END

CLOSE db_cursor
DEALLOCATE db_cursor
```

#### Grant job management permissions in msdb
```sql
USE msdb;

-- Create user if not exists
IF NOT EXISTS (
    SELECT 1 FROM sys.database_principals WHERE name = 'testuser'
)
BEGIN
    CREATE USER [testuser] FOR LOGIN [testuser];
END

-- Grant required permissions
GRANT EXECUTE ON OBJECT::dbo.sp_add_job TO [testuser];
GRANT EXECUTE ON OBJECT::dbo.sp_add_jobstep TO [testuser];
GRANT EXECUTE ON OBJECT::dbo.sp_add_jobschedule TO [testuser];
GRANT EXECUTE ON OBJECT::dbo.sp_add_jobserver TO [testuser];
GRANT EXECUTE ON OBJECT::dbo.sp_update_job TO [testuser];
GRANT EXECUTE ON OBJECT::dbo.sp_add_category TO [testuser];
GRANT EXECUTE ON OBJECT::dbo.sp_delete_job TO [testuser];
```

### Step 3: Install Schema
Run:
```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\aspnet_regsql.exe -S localhost -E -d ALOD -ssadd -sstype c
```
- Use `-E` for Windows Auth if sysadmin. Otherwise, use `-U testuser -P Password123!@#`.
- Confirm with 'y'.

### Step 4: Verify
In SSMS (ALOD database):
- Tables: `ASPStateTempSessions` and `ASPStateTempApplications` exist.
- `Flags` column in `ASPStateTempSessions`: int NOT NULL with DEFAULT (0).
- Procedures like `TempInsertStateItemShort` include `Flags` in INSERTs.

### Step 5: Test
- Run `iisreset` in admin Command Prompt.
- Reload the application and verify sessions work without errors.

## Temporary Hack (If Install Fails)
Alter the table manually (not recommended long-term):
```sql
ALTER TABLE dbo.ASPStateTempSessions ADD CONSTRAINT DF_ASPStateTempSessions_Flags DEFAULT (0) FOR Flags;
```

## Notes
- Ensure web.config has `<sessionState mode="SQLServer" sqlConnectionString="LOD" timeout="31" allowCustomSqlDatabase="true" />`.
- If errors persist, check SQL logs or provide output for further troubleshooting.
- Document last updated: October 7, 2025
