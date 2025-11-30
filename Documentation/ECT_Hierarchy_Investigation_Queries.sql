-- ECT Hierarchy Investigation Queries
-- Prepared for meeting follow-up on Unit Hierarchy Configuration Issues
-- Database: ALOD (ECT System)
-- Server: D-DII-JI01-01\MSSQLSERVER2022

-- =============================================
-- 1. HIERARCHY CONFIGURATION INVESTIGATION
-- =============================================

-- Query 1: Identify tables storing hierarchy and role data
SELECT 
    'Hierarchy Tables' as Category,
    TABLE_NAME as TableName,
    TABLE_TYPE as TableType
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME LIKE '%command%' 
   OR TABLE_NAME LIKE '%hierarchy%'
   OR TABLE_NAME LIKE '%struct%'
   OR TABLE_NAME LIKE '%chain%'
ORDER BY TABLE_NAME;

-- Query 2: Examine command_struct_chain table structure (hierarchy data)
SELECT 
    'command_struct_chain Structure' as Info,
    COLUMN_NAME as ColumnName,
    DATA_TYPE as DataType,
    IS_NULLABLE as Nullable,
    COLUMN_DEFAULT as DefaultValue
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'command_struct_chain'
ORDER BY ORDINAL_POSITION;

-- Query 3: Sample hierarchy data from command_struct_chain
SELECT TOP 20
    csc_id,
    CSC_ID_PARENT,
    CS_ID,
    CHAIN_TYPE,
    --Reportingview,
    CREATED_BY,
    CREATED_DATE,
    MODIFIED_BY,
    MODIFIED_DATE,
    UserModified
FROM command_struct_chain
ORDER BY MODIFIED_DATE DESC;

-- =============================================
-- 2. USER ROLES AND PERMISSIONS INVESTIGATION
-- =============================================

-- Query 4: Examine core_userRoles table structure
SELECT 
    'core_userRoles Structure' as Info,
    COLUMN_NAME as ColumnName,
    DATA_TYPE as DataType,
    IS_NULLABLE as Nullable
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'core_userRoles'
ORDER BY ORDINAL_POSITION;

-- Query 5: Examine core_users table structure
SELECT 
    'core_users Structure' as Info,
    COLUMN_NAME as ColumnName,
    DATA_TYPE as DataType,
    IS_NULLABLE as Nullable
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'core_users'
ORDER BY ORDINAL_POSITION;

-- Query 6: All users with system administrator permissions (groupId = 1)
SELECT 
    u.userId,
    u.username,
    u.FirstName,
    u.LastName,
    u.accessStatus,
    s.description as AccessStatusText,
    g.name as RoleName,
    c.LONG_NAME + ' (' + c.PAS_CODE + ')' as CurrentUnitName,
    u.workCompo,
    u.created_date,
    u.modified_date
FROM core_users u
LEFT JOIN core_lkupAccessStatus s ON s.statusId = u.accessStatus
LEFT JOIN core_UserRoles r ON r.userid = u.userID AND r.active = 1
LEFT JOIN core_UserGroups g ON g.groupId = r.groupId
LEFT JOIN Command_Struct c ON c.CS_ID = ISNULL(u.ada_cs_id, u.cs_id)
WHERE r.groupId = 1  -- SystemAdministrator = 1
ORDER BY u.modified_date DESC;

-- Query 7: Users with access status value of 4 (Disabled) - as mentioned in meeting
SELECT 
    u.userId,
    u.username,
    u.FirstName,
    u.LastName,
    u.accessStatus,
    s.description as AccessStatusText,
    g.name as RoleName,
    c.LONG_NAME + ' (' + c.PAS_CODE + ')' as CurrentUnitName,
    u.workCompo,
    u.created_date,
    u.modified_date
FROM core_users u
LEFT JOIN core_lkupAccessStatus s ON s.statusId = u.accessStatus
LEFT JOIN core_UserRoles r ON r.userid = u.userID AND r.active = 1
LEFT JOIN core_UserGroups g ON g.groupId = r.groupId
LEFT JOIN Command_Struct c ON c.CS_ID = ISNULL(u.ada_cs_id, u.cs_id)
WHERE u.accessStatus = 4  -- Disabled = 4
ORDER BY u.modified_date DESC;

-- Query 8: All administrator-level roles and their users
SELECT 
    g.groupId,
    g.name as RoleName,
    COUNT(r.userId) as UserCount,
    COUNT(CASE WHEN u.accessStatus = 3 THEN 1 END) as ActiveUsers,
    COUNT(CASE WHEN u.accessStatus = 4 THEN 1 END) as DisabledUsers
FROM core_UserGroups g
LEFT JOIN core_UserRoles r ON r.groupId = g.groupId AND r.active = 1
LEFT JOIN core_users u ON u.userId = r.userId
WHERE g.groupId IN (1, 2, 5, 6, 12, 38)  -- Admin-level roles: SystemAdmin, UnitCommander, WingCommander, WingJA, WingAdmin, NafAdmin
GROUP BY g.groupId, g.name
ORDER BY g.groupId;

-- =============================================
-- 3. AUDIT LOGS AND CHANGE TRACKING
-- =============================================

-- Query 9: Check for audit log tables
SELECT 
    'Audit Tables' as Category,
    TABLE_NAME as TableName,
    TABLE_TYPE as TableType
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME LIKE '%audit%' 
   OR TABLE_NAME LIKE '%log%'
   OR TABLE_NAME LIKE '%track%'
   OR TABLE_NAME LIKE '%change%'
ORDER BY TABLE_NAME;

-- Query 10: Examine core_logAction table (audit logs)
SELECT 
    'core_logAction Structure' as Info,
    COLUMN_NAME as ColumnName,
    DATA_TYPE as DataType,
    IS_NULLABLE as Nullable
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'core_logAction'
ORDER BY ORDINAL_POSITION;

-- Query 11: Recent audit log entries for hierarchy changes
SELECT TOP 50
    logId,
    actionDate,
    actionId,
    userId,
    moduleId,
    referenceId,
    notes
FROM core_logAction
WHERE moduleId IN (SELECT moduleId FROM core_lkupModule WHERE moduleName LIKE '%hierarchy%' OR moduleName LIKE '%command%' OR moduleName LIKE '%struct%')
   OR notes LIKE '%hierarchy%'
   OR notes LIKE '%command%'
   OR notes LIKE '%struct%'
ORDER BY actionDate DESC;

-- Query 12: Recent changes to command_struct_chain table
SELECT TOP 20
    csc_id,
    CSC_ID_PARENT,
    CS_ID,
    CHAIN_TYPE,
    CREATED_BY,
    CREATED_DATE,
    MODIFIED_BY,
    MODIFIED_DATE,
    UserModified
FROM command_struct_chain
WHERE MODIFIED_DATE >= DATEADD(day, -300, GETDATE())  -- Last 30 days
ORDER BY MODIFIED_DATE DESC;

-- =============================================
-- 4. IMPORT PROCESS INVESTIGATION
-- =============================================

-- Query 13: Look for stored procedures related to imports
SELECT 
    'Import Procedures' as Category,
    ROUTINE_NAME as ProcedureName,
    ROUTINE_TYPE as ProcedureType,
    CREATED as CreatedDate,
    LAST_ALTERED as LastModified
FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_NAME LIKE '%import%'
   OR ROUTINE_NAME LIKE '%manpower%'
   OR ROUTINE_NAME LIKE '%mill%'
   OR ROUTINE_NAME LIKE '%pds%'
   OR ROUTINE_NAME LIKE '%memberData%'
ORDER BY LAST_ALTERED DESC;

-- Query 14: Check for memberData_Import procedure (found in ALOD_Update2.publish.sql)
SELECT 
    'memberData_Import Details' as Info,
    ROUTINE_NAME as ProcedureName,
    ROUTINE_DEFINITION as ProcedureDefinition
FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_NAME = 'memberData_Import';

-- Query 15: Look for scheduled jobs or tasks (if accessible)
SELECT 
    'Scheduled Jobs' as Category,
    job_id,
    name as JobName,
    enabled,
    date_created,
    date_modified
FROM msdb.dbo.sysjobs
WHERE name LIKE '%import%'
   OR name LIKE '%manpower%'
   OR name LIKE '%mill%'
   OR name LIKE '%pds%'
   OR name LIKE '%memberData%';

-- =============================================
-- 5. 106 RESCUE WING SPECIFIC INVESTIGATION
-- =============================================
-- Query 16: Find 106 Rescue Wing in command structure
SELECT 
    CS_ID,
    PAS_CODE,
    LONG_NAME,
    CS_ID_PARENT,
    CS_LEVEL
FROM Command_Struct
WHERE LONG_NAME LIKE '%106%'
   OR LONG_NAME LIKE '%Rescue%'
   OR PAS_CODE LIKE '%106%'
ORDER BY CS_LEVEL, LONG_NAME;

-- Query 17: Hierarchy chain for 106 Rescue Wing
SELECT 
    csc.csc_id,
    csc.CSC_ID_PARENT,
    csc.CS_ID,
    csc.CHAIN_TYPE,
    cs.LONG_NAME,
    cs.PAS_CODE,
    csc.CREATED_BY,
    csc.CREATED_DATE,
    csc.MODIFIED_BY,
    csc.MODIFIED_DATE
FROM command_struct_chain csc
JOIN Command_Struct cs ON cs.CS_ID = csc.CS_ID
WHERE cs.LONG_NAME LIKE '%106%'
   OR cs.LONG_NAME LIKE '%Rescue%'
   OR cs.PAS_CODE LIKE '%106%'
ORDER BY csc.MODIFIED_DATE DESC;

-- =============================================
-- 6. SUMMARY REPORTS
-- =============================================

-- Query 18: Summary of administrator users by role
SELECT 
    g.name as RoleName,
    COUNT(*) as TotalUsers,
    COUNT(CASE WHEN u.accessStatus = 3 THEN 1 END) as ActiveUsers,
    COUNT(CASE WHEN u.accessStatus = 4 THEN 1 END) as DisabledUsers,
    COUNT(CASE WHEN u.accessStatus = 2 THEN 1 END) as PendingUsers
FROM core_UserGroups g
JOIN core_UserRoles r ON r.groupId = g.groupId AND r.active = 1
JOIN core_users u ON u.userId = r.userId
WHERE g.groupId IN (1, 2, 5, 6, 12, 38)  -- Admin-level roles
GROUP BY g.groupId, g.name
ORDER BY TotalUsers DESC;

-- Query 19: Recent hierarchy modifications summary
SELECT 
    COUNT(*) as TotalModifications,
    COUNT(DISTINCT MODIFIED_BY) as UniqueModifiers,
    MIN(MODIFIED_DATE) as EarliestModification,
    MAX(MODIFIED_DATE) as LatestModification
FROM command_struct_chain
WHERE MODIFIED_DATE >= DATEADD(day, -90, GETDATE());  -- Last 90 days

-- Query 20: Users who have modified hierarchy recently
SELECT 
    MODIFIED_BY as ModifiedBy,
    COUNT(*) as ModificationCount,
    MAX(MODIFIED_DATE) as LastModification
FROM command_struct_chain
WHERE MODIFIED_DATE >= DATEADD(day, -90, GETDATE())
  AND MODIFIED_BY IS NOT NULL
GROUP BY MODIFIED_BY
ORDER BY ModificationCount DESC;
