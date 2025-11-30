# ECT Hierarchy Investigation Action Plan

## Meeting Context
**Date**: Recent meeting with Jacqueline, Jeff, Donell, David, Kristina, and Joe  
**Topic**: Unit Hierarchy Configuration Issues in ECT System  
**Focus**: 106 Rescue Wing misconfigurations and broader hierarchy management

## Action Items Assigned to Joe

### ✅ 1. Hierarchy Configuration Investigation (COMPLETED)
**Task**: Query the SQL database to identify which tables store hierarchy and role data, and determine if audit logs exist for changes to these tables.

**Findings**:
- **Primary Hierarchy Table**: `command_struct_chain` - stores organizational hierarchy relationships
- **User Roles Table**: `core_userRoles` - links users to their roles/groups
- **User Data Table**: `core_users` - contains user information and access status
- **Audit Log Table**: `core_logAction` - tracks system changes and modifications
- **Command Structure Table**: `Command_Struct` - contains unit information and hierarchy

**Key Fields Identified**:
- `command_struct_chain.CHAIN_TYPE` - defines hierarchy type (administrative, reporting, etc.)
- `command_struct_chain.Reportingview` - controls visibility and access
- `core_users.accessStatus` - user access level (1=None, 2=Pending, 3=Approved, 4=Disabled)
- `core_UserRoles.groupId` - links to user groups (1=SystemAdministrator, etc.)

### 🔄 2. Administrator Permissions Audit (IN PROGRESS)
**Task**: Run a database query to generate a report of all user accounts with system administrator permissions in the application.

**SQL Query Created**:
```sql
SELECT 
    u.userId, u.username, u.FirstName, u.LastName, u.accessStatus,
    s.description as AccessStatusText, g.name as RoleName,
    c.LONG_NAME + ' (' + c.PAS_CODE + ')' as CurrentUnitName,
    u.workCompo, u.createdDate, u.modifiedDate
FROM core_users u
LEFT JOIN core_lkupAccessStatus s ON s.statusId = u.accessStatus
LEFT JOIN core_UserRoles r ON r.userid = u.userID AND r.active = 1
LEFT JOIN core_UserGroups g ON g.groupId = r.groupId
LEFT JOIN Command_Struct c ON c.CS_ID = ISNULL(u.ada_cs_id, u.cs_id)
WHERE r.groupId = 1  -- SystemAdministrator = 1
ORDER BY u.modifiedDate DESC;
```

### 🔄 3. Import Process Verification (IN PROGRESS)
**Task**: Investigate scheduled tasks and log files on the database servers to confirm whether an automated import process is updating hierarchy data from external sources such as Manpower or Mill PDS.

**Key Findings**:
- **Stored Procedure Found**: `memberData_Import` - appears to handle bulk data imports
- **Procedure Details**: Located in `ALOD_Update2.publish.sql` lines 6340-6360
- **System Admin Detection**: Procedure automatically finds system admin users for import operations
- **Import Scope**: Handles PAFSC codes, command structure, and member data

**Investigation Queries Created**:
- Search for import-related stored procedures
- Check for scheduled jobs in msdb
- Examine recent modifications to hierarchy tables

### 🔄 4. Access Permissions Review (IN PROGRESS)
**Task**: Query the database to identify all users with administrator access, specifically those with an access status value of four in the core_users table.

**SQL Query Created**:
```sql
SELECT 
    u.userId, u.username, u.FirstName, u.LastName, u.accessStatus,
    s.description as AccessStatusText, g.name as RoleName,
    c.LONG_NAME + ' (' + c.PAS_CODE + ')' as CurrentUnitName,
    u.workCompo, u.createdDate, u.modifiedDate
FROM core_users u
LEFT JOIN core_lkupAccessStatus s ON s.statusId = u.accessStatus
LEFT JOIN core_UserRoles r ON r.userid = u.userID AND r.active = 1
LEFT JOIN core_UserGroups g ON g.groupId = r.groupId
LEFT JOIN Command_Struct c ON c.CS_ID = ISNULL(u.ada_cs_id, u.cs_id)
WHERE u.accessStatus = 4  -- Disabled = 4
ORDER BY u.modifiedDate DESC;
```

### 📋 5. Technical Follow-Up Meeting Preparation (PENDING)
**Task**: Schedule a follow-up meeting early next week to review progress on the investigation and discuss findings related to hierarchy configuration, audit logs, and administrator access.

## Tools Created for Investigation

### 1. SQL Query File: `ECT_Hierarchy_Investigation_Queries.sql`
**Purpose**: Comprehensive SQL queries to investigate all aspects of the hierarchy issues
**Contents**:
- 20 detailed queries covering all investigation areas
- Hierarchy table structure analysis
- Administrator permissions audit
- Recent changes tracking
- 106 Rescue Wing specific investigation
- Import process analysis
- Summary reports

### 2. PowerShell Script: `ECT_Hierarchy_Investigation_Script.ps1`
**Purpose**: Automated script to execute queries and generate reports
**Features**:
- Connects to database using Windows Authentication
- Executes all investigation queries
- Generates formatted reports
- Creates summary documentation
- Handles errors gracefully

## Database Connection Information
- **Server**: D-DII-JI01-01\MSSQLSERVER2022
- **Database**: ALOD
- **Authentication**: Windows Authentication (Trusted_Connection=True)
- **Connection String**: Available in `ALOD/configFiles/connectionStrings.config`

## Key Database Tables Identified

### Hierarchy Management
- `command_struct_chain` - Primary hierarchy relationships
- `Command_Struct` - Unit information and structure
- `Command_Struct_Tree` - Hierarchical tree views

### User Management
- `core_users` - User accounts and basic information
- `core_UserRoles` - User-to-role assignments
- `core_UserGroups` - Role definitions and permissions
- `core_GroupPermissions` - Detailed permission mappings

### Audit and Logging
- `core_logAction` - System change tracking
- `core_logActionChanges` - Detailed change records

## Next Steps

### Immediate Actions (This Week)
1. **Execute Investigation Script**: Run the PowerShell script to generate reports
2. **Review Generated Reports**: Analyze the output files for specific issues
3. **Identify Misconfigurations**: Focus on 106 Rescue Wing specific problems
4. **Document Findings**: Prepare detailed findings for team review

### Follow-Up Actions (Next Week)
1. **Team Meeting**: Present findings to Jacqueline, Jeff, Donell, David, and Kristina
2. **Root Cause Analysis**: Determine if issues are from manual entry or automated imports
3. **Corrective Actions**: Implement fixes for identified misconfigurations
4. **Process Improvements**: Establish better controls for hierarchy management

## Risk Mitigation
- **Backup Strategy**: Ensure database backups before making any changes
- **Change Control**: Document all modifications with proper approval
- **Testing**: Test changes in development environment first
- **Monitoring**: Implement additional logging for future changes

## Success Criteria
- [ ] Complete administrator permissions audit
- [ ] Identify source of 106 Rescue Wing misconfigurations
- [ ] Determine if automated imports are causing issues
- [ ] Present findings to team in follow-up meeting
- [ ] Implement corrective actions based on findings
- [ ] Establish improved hierarchy management processes

## Contact Information
- **Primary Contact**: Kristina (meeting coordinator)
- **Technical Lead**: David (database investigation)
- **Subject Matter Expert**: Jacqueline (hierarchy configuration)
- **System Administrator**: Jeff (technical implementation)
