# ALOD Database Backup and Migration Plan

This document outlines a plan to backup and migrate the ALOD database from a local SQL Server 2022 instance to a test environment running SQL Server 2016. Since direct .bak backups are not backward-compatible, we use scripting or BACPAC exports as workarounds. The primary method is SSMS Generate Scripts, which avoids Azure validation issues in BACPAC. Always test on a database copy and backup your original DB first.

## Prerequisites
- **SQL Server Management Studio (SSMS):** Version 18+ (download from [Microsoft](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) if needed).
- **Access to Both Servers:** Local 2022 instance and remote 2016 test server (with login credentials).
- **Backup Original DB:** Right-click DB in SSMS > Tasks > Back Up > Full backup to .bak.
- **Tools:** Optional sqlpackage.exe (found in C:\Program Files\Microsoft SQL Server\*\DAC\bin).
- **Project Awareness:** Your setup includes ALOD.Database.sqlproj (SSDT project) and related files like connectionStrings.config—update configs post-migration.
- **Time Estimate:** 30-60 minutes for small DBs; longer for large data sets.

## Recommended Approach: Generate Scripts in SSMS
This method scripts schema and data, targeting SQL Server 2016 directly. It's simpler and avoids BACPAC's Azure compatibility checks.

### Step 1: Generate Scripts on Local Machine (SQL 2022)
1. Open SSMS and connect to your local SQL Server 2022.
2. Right-click the database (e.g., ALOD) > Tasks > Generate Scripts.
3. Wizard Steps:
   - **Choose Objects:** Select "Script entire database and all database objects" (or exclude problematic items like users/certificates).
   - **Set Scripting Options:**
     - Save to a single file (e.g., C:\Temp\ALOD_Script.sql).
     - Advanced:
       - Script for Server Version: SQL Server 2016.
       - Types of data to script: Schema and data.
       - Script Logins: False.
       - Script Users: False (skips orphaned users).
       - Script Certificates: False.
       - Check for object existence: True.
     - If the DB is large, set "Script data" to False initially (handle data separately—see Data Migration Tips below).
4. Click Finish to generate the .sql file.

### Step 2: Execute Scripts on Test Server (SQL 2016)
1. Copy the .sql file to the test environment.
2. In SSMS, connect to the 2016 server.
3. Create a new empty database: Right-click Databases > New Database > Name it (e.g., ALOD_Test).
4. Open the .sql file and execute (F5). Monitor the output for errors.
5. If errors occur (e.g., cross-db references), edit the .sql file:
   - Comment out problematic sections with `--` (e.g., procedures referencing [alod].[dbo] or [msdb].[dbo]).
   - Rerun.

## Alternative Approach: Fix and Export BACPAC
Use if Generate Scripts fails. BACPAC exports to a portable file but requires fixing Azure-incompatible elements.

### Step 1: Fix Issues in Local DB Copy
Run these in SSMS on a DB copy:

- **Drop Orphaned Users:**
  ```sql
  DROP USER [ALOD_ASPNET_SESSION];
  DROP USER [DBSign];
  DROP USER [ECT];
  DROP USER [ALODTEST!];
  DROP USER [PageLog];
  -- Windows auth users
  DROP USER [UHHZ-DB-009V\Administrator];
  DROP USER [AREA52\$svc.uhhz.alod];
  DROP USER [NT AUTHORITY\SYSTEM];
  -- If schema ownership error: ALTER AUTHORIZATION ON SCHEMA::DBSign TO dbo;
  ```

- **Drop Certificates:**
  ```sql
  DROP CERTIFICATE [ALODServerCertProd];
  DROP CERTIFICATE [ALODServerCertProd2];
  ```

- **Drop/Fix Unresolved Views:**
  ```sql
  DROP VIEW [dbo].[vw_sc];
  DROP VIEW [dbo].[vw_lod_348_audit];
  -- Or script and edit to fix references (e.g., remove [alod].[dbo] prefixes).
  ```

- **Drop Cross-DB Procedures:**
  ```sql
  DROP PROCEDURE [dbo].[ReminderGetUsersToDisable];
  DROP PROCEDURE [dbo].[CreateTempTables];
  DROP PROCEDURE [dbo].[memo_sp_PSC_Determination];
  DROP PROCEDURE [dbo].[TempGetAppID];  -- And other Temp* procs
  DROP PROCEDURE [dbo].[core_email_sp_SendWaiverReminders];
  -- List from errors and drop others as needed.
  ```

### Step 2: Export BACPAC
- In SSMS: Right-click DB > Tasks > Export Data-tier Application > Save .bacpac.
- Or via sqlpackage.exe:
  ```
  sqlpackage.exe /Action:Export /TargetFile:"C:\Temp\ALOD.bacpac" /SourceServerName:"localhost" /SourceDatabaseName:"ALOD" /SourceUser:"sa" /SourcePassword:"your_password" /p:IgnoreUserSettingsObjects=true /p:IgnorePermissions=true /p:IgnoreLoginSids=true
  ```

### Step 3: Import BACPAC on Test Server
- In SSMS: Right-click Databases > Import Data-tier Application > Select .bacpac > Create new DB.

## Data Migration Tips
- **Large Datasets:** If scripting data is slow, use:
  - **BCP Tool:** Export/import tables (e.g., `bcp "SELECT * FROM table" queryout "file.dat" -S server -T -c`).
  - **SSIS:** Use SQL Server Integration Services for bulk transfer (create a simple package in Visual Studio).
- **Exclude Temp/ASPState Tables:** If using ASP.NET session state in SQL, exclude or recreate manually.

## Troubleshooting
- **Common Errors:**
  - Unresolved References: Ensure all tables/views exist; drop/recreate affected objects.
  - Cross-DB Refs: Rewrite procs to use local data or synonyms.
  - Version Mismatches: Check for 2022-specific features (e.g., new T-SQL syntax) and adjust.
- **Logs:** Check SSMS Messages tab or SQL Server Error Log (Object Explorer > SQL Server Logs).
- **If Stuck:** Run `SELECT @@VERSION` on both servers to confirm versions. Share specific error messages for help.

## Verification Steps Post-Migration
1. Query key tables: `SELECT TOP 10 * FROM core_Users;` (adjust for your tables).
2. Test app connections: Update connection strings in files like connectionStrings.config and test your app (e.g., SRXDocumentStore.cs seems data-related).
3. Compare Counts: Run `SELECT COUNT(*) FROM table` on both DBs.
4. Recreate Dropped Objects: If you dropped users/procs, add them back on the test DB as needed.

## Additional Notes
- **Security:** Reset passwords and permissions after migration.
- **Project Integration:** Open ALOD.Databases.sln in Visual Studio, set Target Platform to SQL 2016, and publish directly if possible.
- **Backup Frequency:** Schedule regular .bak backups on 2022; use this plan for future migrations.
- **If Upgrading Test Server:** Consider upgrading to 2022 for direct .bak compatibility.

For questions, contact your DBA or provide more details (e.g., error logs).
