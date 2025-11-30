<#
.SYNOPSIS
    Verifies the correct installation of the ALOD website in Testing configuration on IIS.

.DESCRIPTION
    This script performs comprehensive verification of the ALOD website deployment to IIS
    in the Testing configuration. It validates 50+ critical checks across multiple categories:
    
    - IIS Configuration: Service status, physical path, critical files
    - Application Pool: State, .NET version (v4.0), pipeline mode, identity settings
    - Web.config Settings: Connection strings, Testing-specific app settings, SMTP, session state
    - Database Connectivity: SQL Server authentication to ALOD and SRXLite databases
    - SQL Server Login: Existence, enabled status, default database
    - User Permissions: Database role memberships and object permissions
    - Session State: ASP.NET session state tables (ASPStateTempApplications, ASPStateTempSessions)
    
    The script validates against settings defined in web.Testing.config to ensure proper
    deployment and configuration for the Testing environment with:
    - Remote SQL Server instance (uhhz-db-009v.area52.afnoapps.usaf.mil\MSSQLSERVER2014)
    - SQL Server authentication (ALOD_ASPNET_SESSION user)
    - DEV deployment mode (Testing configuration)
    - Remote SMTP server (131.9.25.144)
    - Testing hostname (alodtest.afrc.af.mil)

.PARAMETER IISPath
    The IIS physical path where ALOD is deployed. Defaults to C:\inetpub\ALOD

.PARAMETER SiteName
    The IIS site name. Defaults to "Default Web Site"

.PARAMETER AppName
    The IIS application name. Defaults to "ALOD"

.PARAMETER Detailed
    If specified, provides detailed output for all checks including informational messages

.EXAMPLE
    .\Verify-ALODTestingInstallation.ps1
    Runs basic verification checks with default settings. Shows pass/fail/warning status
    for all checks with summary statistics.

.EXAMPLE
    .\Verify-ALODTestingInstallation.ps1 -Detailed
    Runs verification with detailed output including INFO messages for all configuration
    values detected (SQL Server version, connection strings, app settings, etc.).

.EXAMPLE
    .\Verify-ALODTestingInstallation.ps1 -IISPath "D:\inetpub\ALOD" -SiteName "ALODTest"
    Runs verification with custom IIS path and site name for non-standard deployments.

.OUTPUTS
    Exit Code 0: All checks passed (or passed with warnings only)
    Exit Code 1: One or more critical checks failed

.NOTES
    Author: ALOD Development Team
    Date: November 21, 2025
    Version: 1.0
    Requires: PowerShell 5.1+, IIS with WebAdministration module, SqlServer module
    Configuration: Testing environment (web.Testing.config)
    
    Prerequisites:
    - IIS must be installed and W3SVC service running
    - WebAdministration module (built-in with IIS)
    - SqlServer PowerShell module (install via: Install-Module SqlServer)
    - ALOD website deployed to IIS physical path
    - SQL Server accessible on remote server with SQL authentication
    - ALOD_ASPNET_SESSION SQL login must exist with correct database permissions
    
    Known Warnings (Expected):
    - User may be missing db_datareader/db_datawriter if user has db_owner role (db_owner includes these)
    - Session state tables warning if not yet created (run aspnet_regsql.exe to create)
    
    Administrator Privileges:
    - Script can run without elevation but some IIS provider checks work best with admin rights
    - If not elevated, a warning is displayed but most checks (50+) will still execute successfully
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory=$false)]
    [string]$IISPath = "C:\inetpub\ALOD",
    
    [Parameter(Mandatory=$false)]
    [string]$SiteName = "Default Web Site",
    
    [Parameter(Mandatory=$false)]
    [string]$AppName = "ALOD",
    
    [Parameter(Mandatory=$false)]
    [switch]$Detailed
)

# Testing configuration values from web.Testing.config
$TestingConfig = @{
    DatabaseServer = "uhhz-db-009v.area52.afnoapps.usaf.mil\MSSQLSERVER2014"
    DatabaseName = "ALOD"
    DatabaseUser = "ALOD_ASPNET_SESSION"
    DatabasePassword = "alodpwd!2009"
    SRXDatabaseName = "SRXLite"
    SMTPServer = "131.9.25.144"
    SessionTimeout = 31
    ExpectedAppPoolIdentity = "ApplicationPoolIdentity" # or specific account
}

# Script-level variables
$script:ErrorCount = 0
$script:WarningCount = 0
$script:SuccessCount = 0
$script:Results = @()

# Color-coded output functions
function Write-CheckHeader {
    param([string]$Message)
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host $Message -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "[PASS] $Message" -ForegroundColor Green
    $script:SuccessCount++
    $script:Results += [PSCustomObject]@{
        Status = "PASS"
        Message = $Message
        Details = ""
    }
}

function Write-Warning {
    param(
        [string]$Message,
        [string]$Details = ""
    )
    Write-Host "[WARN] $Message" -ForegroundColor Yellow
    if ($Details -and $Detailed) {
        Write-Host "       $Details" -ForegroundColor DarkYellow
    }
    $script:WarningCount++
    $script:Results += [PSCustomObject]@{
        Status = "WARN"
        Message = $Message
        Details = $Details
    }
}

function Write-Failure {
    param(
        [string]$Message,
        [string]$Details = ""
    )
    Write-Host "[FAIL] $Message" -ForegroundColor Red
    if ($Details -and $Detailed) {
        Write-Host "       $Details" -ForegroundColor DarkRed
    }
    $script:ErrorCount++
    $script:Results += [PSCustomObject]@{
        Status = "FAIL"
        Message = $Message
        Details = $Details
    }
}

function Write-Info {
    param([string]$Message)
    if ($Detailed) {
        Write-Host "[INFO] $Message" -ForegroundColor Cyan
    }
}

# Check if running as administrator
function Test-Administrator {
    $currentUser = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($currentUser)
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

# Load required modules
function Initialize-RequiredModules {
    Write-CheckHeader "Initializing Required Modules"
    
    # Check for WebAdministration module
    if (Get-Module -ListAvailable -Name WebAdministration) {
        Import-Module WebAdministration -ErrorAction SilentlyContinue
        if ($?) {
            Write-Success "WebAdministration module loaded"
        } else {
            Write-Failure "Failed to import WebAdministration module"
            return $false
        }
    } else {
        Write-Failure "WebAdministration module not found. IIS may not be installed."
        return $false
    }
    
    # Check for SqlServer module
    if (Get-Module -ListAvailable -Name SqlServer) {
        Import-Module SqlServer -ErrorAction SilentlyContinue
        if ($?) {
            Write-Success "SqlServer module loaded"
        } else {
            Write-Warning "Failed to import SqlServer module. Database checks may be limited." `
                "Install with: Install-Module -Name SqlServer -Force"
        }
    } else {
        Write-Warning "SqlServer module not found. Some database checks will be skipped." `
            "Install with: Install-Module -Name SqlServer -Force"
    }
    
    return $true
}

# Verify IIS installation and site
function Test-IISConfiguration {
    Write-CheckHeader "Verifying IIS Configuration"
    
    # Check if IIS is installed and running
    $iisService = Get-Service -Name W3SVC -ErrorAction SilentlyContinue
    if ($null -eq $iisService) {
        Write-Failure "IIS (W3SVC) service not found. IIS may not be installed."
        return $false
    }
    
    if ($iisService.Status -ne "Running") {
        Write-Failure "IIS service is not running. Current status: $($iisService.Status)"
        return $false
    } else {
        Write-Success "IIS service is running"
    }
    
    # Check if physical path exists
    if (Test-Path $IISPath) {
        Write-Success "IIS physical path exists: $IISPath"
        
        # Check for critical files
        $criticalFiles = @(
            "web.config",
            "default.aspx",
            "Global.asax"
        )
        
        foreach ($file in $criticalFiles) {
            $filePath = Join-Path $IISPath $file
            if (Test-Path $filePath) {
                Write-Success "Critical file found: $file"
            } else {
                Write-Failure "Critical file missing: $file"
            }
        }
        
        # Check for bin directory and assemblies
        $binPath = Join-Path $IISPath "bin"
        if (Test-Path $binPath) {
            Write-Success "bin directory exists"
            
            $requiredDlls = @(
                "ALOD.Core.dll",
                "ALOD.Data.dll",
                "ALODWebUtility.dll",
                "NHibernate.dll",
                "ABCpdf.dll"
            )
            
            foreach ($dll in $requiredDlls) {
                $dllPath = Join-Path $binPath $dll
                if (Test-Path $dllPath) {
                    Write-Success "Required assembly found: $dll"
                } else {
                    Write-Failure "Required assembly missing: $dll"
                }
            }
        } else {
            Write-Failure "bin directory not found"
        }
    } else {
        Write-Failure "IIS physical path does not exist: $IISPath"
        return $false
    }
    
    # Check IIS application
    try {
        $app = Get-WebApplication -Site $SiteName -Name $AppName -ErrorAction SilentlyContinue
        if ($null -ne $app) {
            Write-Success "IIS application found: $AppName"
            Write-Info "Physical Path: $($app.PhysicalPath)"
            Write-Info "Application Pool: $($app.ApplicationPool)"
            
            # Verify physical path matches
            if ($app.PhysicalPath -ne $IISPath) {
                Write-Warning "IIS application physical path ($($app.PhysicalPath)) does not match expected path ($IISPath)"
            }
        } else {
            Write-Warning "IIS application not found: $AppName. Site may be at root."
        }
    } catch {
        Write-Warning "Error checking IIS application: $($_.Exception.Message)"
    }
    
    return $true
}

# Verify Application Pool configuration
function Test-ApplicationPool {
    Write-CheckHeader "Verifying Application Pool Configuration"
    
    try {
        # Get application pool name from site or app
        $app = Get-WebApplication -Site $SiteName -Name $AppName -ErrorAction SilentlyContinue
        if ($null -eq $app) {
            $site = Get-Website -Name $SiteName
            $appPoolName = $site.ApplicationPool
        } else {
            $appPoolName = $app.ApplicationPool
        }
        
        Write-Info "Application Pool Name: $appPoolName"
        
        # Get application pool details
        $appPool = Get-Item "IIS:\AppPools\$appPoolName" -ErrorAction Stop
        
        if ($null -ne $appPool) {
            Write-Success "Application pool exists: $appPoolName"
            
            # Check state
            if ($appPool.State -ne "Started") {
                Write-Failure "Application pool is not started. Current state: $($appPool.State)"
            } else {
                Write-Success "Application pool is started"
            }
            
            # Check .NET version
            $managedRuntimeVersion = $appPool.managedRuntimeVersion
            Write-Info ".NET Runtime Version: $managedRuntimeVersion"
            if ($managedRuntimeVersion -ne "v4.0") {
                Write-Warning "Application pool runtime version is $managedRuntimeVersion. Expected v4.0 for .NET 4.8.1"
            } else {
                Write-Success "Application pool runtime version is correct (v4.0)"
            }
            
            # Check pipeline mode
            $pipelineMode = $appPool.managedPipelineMode
            Write-Info "Pipeline Mode: $pipelineMode"
            if ($pipelineMode -ne "Integrated") {
                Write-Warning "Application pool pipeline mode is $pipelineMode. Integrated mode is recommended."
            } else {
                Write-Success "Application pool pipeline mode is Integrated"
            }
            
            # Check identity
            $identityType = $appPool.processModel.identityType
            Write-Info "Identity Type: $identityType"
            Write-Success "Application pool identity: $identityType"
            
            # Check 32-bit mode
            $enable32Bit = $appPool.enable32BitAppOnWin64
            Write-Info "Enable 32-bit Applications: $enable32Bit"
            if ($enable32Bit) {
                Write-Warning "32-bit mode is enabled. This may be required for ABCpdf or other dependencies."
            }
            
        } else {
            Write-Failure "Application pool not found: $appPoolName"
            return $false
        }
    } catch {
        Write-Failure "Error checking application pool: $($_.Exception.Message)" $_.Exception.StackTrace
        return $false
    }
    
    return $true
}

# Verify web.config and Testing configuration
function Test-WebConfig {
    Write-CheckHeader "Verifying web.config Configuration"
    
    $webConfigPath = Join-Path $IISPath "web.config"
    
    if (-not (Test-Path $webConfigPath)) {
        Write-Failure "web.config not found at: $webConfigPath"
        return $false
    }
    
    Write-Success "web.config file exists"
    
    try {
        [xml]$webConfig = Get-Content $webConfigPath
        
        # Check connection strings
        $connectionStrings = $webConfig.configuration.connectionStrings
        if ($null -ne $connectionStrings) {
            Write-Success "connectionStrings section exists"
            
            # Check LOD connection string
            $lodConnection = $connectionStrings.add | Where-Object { $_.name -eq "LOD" }
            if ($null -ne $lodConnection) {
                Write-Success "LOD connection string found"
                
                # Verify it contains Testing server
                if ($lodConnection.connectionString -like "*$($TestingConfig.DatabaseServer)*") {
                    Write-Success "LOD connection string uses correct Testing server"
                } else {
                    Write-Failure "LOD connection string does not match Testing server. Expected: $($TestingConfig.DatabaseServer)"
                    Write-Info "Actual: $($lodConnection.connectionString)"
                }
                
                # Verify database name
                if ($lodConnection.connectionString -like "*$($TestingConfig.DatabaseName)*") {
                    Write-Success "LOD connection string uses correct database name"
                } else {
                    Write-Warning "LOD connection string may not use correct database name. Expected: $($TestingConfig.DatabaseName)"
                }
                
                # Verify SQL authentication user
                if ($lodConnection.connectionString -like "*$($TestingConfig.DatabaseUser)*") {
                    Write-Success "LOD connection string uses correct SQL user"
                } else {
                    Write-Failure "LOD connection string does not use correct SQL user. Expected: $($TestingConfig.DatabaseUser)"
                }
            } else {
                Write-Failure "LOD connection string not found in web.config"
            }
            
            # Check SRXLite connection string
            $srxConnection = $connectionStrings.add | Where-Object { $_.name -eq "SRXLite" }
            if ($null -ne $srxConnection) {
                Write-Success "SRXLite connection string found"
                
                if ($srxConnection.connectionString -like "*$($TestingConfig.SRXDatabaseName)*") {
                    Write-Success "SRXLite connection string uses correct database name"
                } else {
                    Write-Warning "SRXLite connection string may not use correct database name. Expected: $($TestingConfig.SRXDatabaseName)"
                }
            } else {
                Write-Warning "SRXLite connection string not found in web.config"
            }
        } else {
            Write-Failure "connectionStrings section not found in web.config"
        }
        
        # Check appSettings for Testing-specific values
        $appSettings = $webConfig.configuration.appSettings
        if ($null -ne $appSettings) {
            Write-Success "appSettings section exists"
            
            # Check DeployMode
            $deployMode = ($appSettings.add | Where-Object { $_.key -eq "DeployMode" }).value
            if ($deployMode -eq "DEV") {
                Write-Success "DeployMode is set to DEV (Testing configuration)"
            } else {
                Write-Warning "DeployMode is '$deployMode'. Expected 'DEV' for Testing."
            }
            
            # Check hostname
            $hostname = ($appSettings.add | Where-Object { $_.key -eq "hostname" }).value
            if ($hostname -like "*alodtest.afrc.af.mil*") {
                Write-Success "hostname is set to Testing URL"
            } else {
                Write-Warning "hostname may not be configured for Testing: $hostname"
            }
            
            # Check email settings
            $emailEnabled = ($appSettings.add | Where-Object { $_.key -eq "EmailEnabled" }).value
            Write-Info "EmailEnabled: $emailEnabled"
        }
        
        # Check system.net SMTP configuration
        $systemNet = $webConfig.configuration.'system.net'
        if ($null -ne $systemNet) {
            $smtpHost = $systemNet.mailSettings.smtp.network.host
            Write-Info "SMTP Host: $smtpHost"
            if ($smtpHost -eq $TestingConfig.SMTPServer) {
                Write-Success "SMTP server is configured for Testing"
            } else {
                Write-Warning "SMTP server is '$smtpHost'. Expected '$($TestingConfig.SMTPServer)' for Testing."
            }
        }
        
        # Check session state configuration
        $sessionState = $webConfig.configuration.'system.web'.sessionState
        if ($null -ne $sessionState) {
            Write-Info "Session State Mode: $($sessionState.mode)"
            Write-Info "Session Timeout: $($sessionState.timeout) minutes"
            
            if ($sessionState.mode -eq "SQLServer") {
                Write-Success "Session state is configured for SQL Server"
                
                if ($sessionState.timeout -eq $TestingConfig.SessionTimeout.ToString()) {
                    Write-Success "Session timeout is configured correctly ($($TestingConfig.SessionTimeout) minutes)"
                } else {
                    Write-Warning "Session timeout is $($sessionState.timeout) minutes. Expected $($TestingConfig.SessionTimeout) minutes for Testing."
                }
            } else {
                Write-Warning "Session state mode is $($sessionState.mode). Expected SQLServer for Testing."
            }
        }
        
    } catch {
        Write-Failure "Error parsing web.config: $($_.Exception.Message)" $_.Exception.StackTrace
        return $false
    }
    
    return $true
}

# Test database connectivity
function Test-DatabaseConnectivity {
    Write-CheckHeader "Testing Database Connectivity"
    
    # Build connection string
    $connectionString = "Server=$($TestingConfig.DatabaseServer);Database=$($TestingConfig.DatabaseName);User Id=$($TestingConfig.DatabaseUser);Password=$($TestingConfig.DatabasePassword);TrustServerCertificate=True;"
    
    try {
        Write-Info "Attempting to connect to: $($TestingConfig.DatabaseServer)"
        Write-Info "Database: $($TestingConfig.DatabaseName)"
        Write-Info "User: $($TestingConfig.DatabaseUser)"
        
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $connection.Open()
        
        if ($connection.State -eq "Open") {
            Write-Success "Successfully connected to ALOD database"
            
            # Get SQL Server version
            $command = $connection.CreateCommand()
            $command.CommandText = "SELECT @@VERSION AS Version"
            $reader = $command.ExecuteReader()
            if ($reader.Read()) {
                $version = $reader["Version"]
                Write-Info "SQL Server Version: $version"
            }
            $reader.Close()
            
            $connection.Close()
        } else {
            Write-Failure "Failed to connect to database. Connection state: $($connection.State)"
            return $false
        }
    } catch {
        Write-Failure "Database connection failed: $($_.Exception.Message)" $_.Exception.StackTrace
        return $false
    }
    
    # Test SRXLite database
    try {
        $srxConnectionString = "Server=$($TestingConfig.DatabaseServer);Database=$($TestingConfig.SRXDatabaseName);User Id=$($TestingConfig.DatabaseUser);Password=$($TestingConfig.DatabasePassword);TrustServerCertificate=True;"
        
        $connection = New-Object System.Data.SqlClient.SqlConnection($srxConnectionString)
        $connection.Open()
        
        if ($connection.State -eq "Open") {
            Write-Success "Successfully connected to SRXLite database"
            $connection.Close()
        } else {
            Write-Warning "Failed to connect to SRXLite database. Connection state: $($connection.State)"
        }
    } catch {
        Write-Warning "SRXLite database connection failed: $($_.Exception.Message)"
    }
    
    return $true
}

# Verify SQL Server login exists
function Test-SqlLogin {
    Write-CheckHeader "Verifying SQL Server Login"
    
    $connectionString = "Server=$($TestingConfig.DatabaseServer);Database=master;User Id=$($TestingConfig.DatabaseUser);Password=$($TestingConfig.DatabasePassword);TrustServerCertificate=True;"
    
    try {
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $connection.Open()
        
        $query = @"
SELECT 
    name,
    type_desc,
    is_disabled,
    create_date,
    modify_date,
    default_database_name
FROM sys.server_principals
WHERE name = '$($TestingConfig.DatabaseUser)'
    AND type IN ('S', 'U')
"@
        
        $command = $connection.CreateCommand()
        $command.CommandText = $query
        $reader = $command.ExecuteReader()
        
        if ($reader.Read()) {
            Write-Success "SQL Server login exists: $($TestingConfig.DatabaseUser)"
            Write-Info "Login Type: $($reader['type_desc'])"
            Write-Info "Disabled: $($reader['is_disabled'])"
            Write-Info "Default Database: $($reader['default_database_name'])"
            Write-Info "Created: $($reader['create_date'])"
            
            if ($reader['is_disabled'] -eq $true) {
                Write-Failure "SQL Server login is disabled"
            } else {
                Write-Success "SQL Server login is enabled"
            }
            
            $reader.Close()
        } else {
            $reader.Close()
            Write-Failure "SQL Server login not found: $($TestingConfig.DatabaseUser)"
            $connection.Close()
            return $false
        }
        
        $connection.Close()
    } catch {
        Write-Failure "Error checking SQL Server login: $($_.Exception.Message)" $_.Exception.StackTrace
        return $false
    }
    
    return $true
}

# Verify database user and permissions
function Test-DatabaseUser {
    Write-CheckHeader "Verifying Database User and Permissions"
    
    $connectionString = "Server=$($TestingConfig.DatabaseServer);Database=$($TestingConfig.DatabaseName);User Id=$($TestingConfig.DatabaseUser);Password=$($TestingConfig.DatabasePassword);TrustServerCertificate=True;"
    
    try {
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $connection.Open()
        
        # Check if database user exists
        $query = @"
SELECT 
    dp.name AS UserName,
    dp.type_desc AS UserType,
    dp.create_date,
    dp.modify_date,
    dp.default_schema_name
FROM sys.database_principals dp
WHERE dp.name = '$($TestingConfig.DatabaseUser)'
    AND dp.type IN ('S', 'U')
"@
        
        $command = $connection.CreateCommand()
        $command.CommandText = $query
        $reader = $command.ExecuteReader()
        
        if ($reader.Read()) {
            Write-Success "Database user exists in $($TestingConfig.DatabaseName): $($TestingConfig.DatabaseUser)"
            Write-Info "User Type: $($reader['UserType'])"
            Write-Info "Default Schema: $($reader['default_schema_name'])"
            Write-Info "Created: $($reader['create_date'])"
            $reader.Close()
        } else {
            $reader.Close()
            Write-Failure "Database user not found in $($TestingConfig.DatabaseName): $($TestingConfig.DatabaseUser)"
            $connection.Close()
            return $false
        }
        
        # Check database role memberships
        $query = @"
SELECT 
    dp.name AS DatabaseRole
FROM sys.database_role_members drm
INNER JOIN sys.database_principals dp ON drm.role_principal_id = dp.principal_id
INNER JOIN sys.database_principals dp2 ON drm.member_principal_id = dp2.principal_id
WHERE dp2.name = '$($TestingConfig.DatabaseUser)'
ORDER BY dp.name
"@
        
        $command = $connection.CreateCommand()
        $command.CommandText = $query
        $reader = $command.ExecuteReader()
        
        $roles = @()
        while ($reader.Read()) {
            $roles += $reader["DatabaseRole"]
        }
        $reader.Close()
        
        if ($roles.Count -gt 0) {
            Write-Success "Database user has role memberships:"
            foreach ($role in $roles) {
                Write-Info "  - $role"
            }
            
            # Check for critical roles
            $requiredRoles = @("db_datareader", "db_datawriter")
            foreach ($role in $requiredRoles) {
                if ($roles -contains $role) {
                    Write-Success "User has required role: $role"
                } else {
                    Write-Warning "User may be missing role: $role"
                }
            }
        } else {
            Write-Warning "Database user has no role memberships. This may cause permission issues."
        }
        
        # Check specific object permissions
        $query = @"
SELECT 
    OBJECT_NAME(major_id) AS ObjectName,
    permission_name AS Permission,
    state_desc AS State
FROM sys.database_permissions
WHERE grantee_principal_id = (
    SELECT principal_id 
    FROM sys.database_principals 
    WHERE name = '$($TestingConfig.DatabaseUser)'
)
    AND major_id > 0
ORDER BY ObjectName, Permission
"@
        
        $command = $connection.CreateCommand()
        $command.CommandText = $query
        $reader = $command.ExecuteReader()
        
        $hasPermissions = $false
        while ($reader.Read()) {
            if (-not $hasPermissions) {
                Write-Info "Specific object permissions:"
                $hasPermissions = $true
            }
            Write-Info "  - $($reader['ObjectName']): $($reader['Permission']) ($($reader['State']))"
        }
        $reader.Close()
        
        if ($hasPermissions) {
            Write-Success "Database user has specific object permissions"
        }
        
        $connection.Close()
    } catch {
        Write-Failure "Error checking database user: $($_.Exception.Message)" $_.Exception.StackTrace
        return $false
    }
    
    return $true
}

# Verify session state database tables
function Test-SessionStateTables {
    Write-CheckHeader "Verifying ASP.NET Session State Tables"
    
    $connectionString = "Server=$($TestingConfig.DatabaseServer);Database=$($TestingConfig.DatabaseName);User Id=$($TestingConfig.DatabaseUser);Password=$($TestingConfig.DatabasePassword);TrustServerCertificate=True;"
    
    try {
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $connection.Open()
        
        # Check for ASP.NET session state tables
        $sessionTables = @(
            "ASPStateTempApplications",
            "ASPStateTempSessions"
        )
        
        $query = @"
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME IN ('ASPStateTempApplications', 'ASPStateTempSessions')
ORDER BY TABLE_NAME
"@
        
        $command = $connection.CreateCommand()
        $command.CommandText = $query
        $reader = $command.ExecuteReader()
        
        $foundTables = @()
        while ($reader.Read()) {
            $foundTables += $reader["TABLE_NAME"]
        }
        $reader.Close()
        
        foreach ($table in $sessionTables) {
            if ($foundTables -contains $table) {
                Write-Success "Session state table exists: $table"
            } else {
                Write-Warning "Session state table not found: $table. Run aspnet_regsql.exe to create session state tables."
            }
        }
        
        $connection.Close()
    } catch {
        Write-Warning "Error checking session state tables: $($_.Exception.Message)"
    }
    
    return $true
}

# Generate summary report
function Show-Summary {
    Write-Host "`n" -NoNewline
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "VERIFICATION SUMMARY" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    
    Write-Host "Passed Checks:  " -NoNewline
    Write-Host $script:SuccessCount -ForegroundColor Green
    
    Write-Host "Warnings:       " -NoNewline
    Write-Host $script:WarningCount -ForegroundColor Yellow
    
    Write-Host "Failed Checks:  " -NoNewline
    Write-Host $script:ErrorCount -ForegroundColor Red
    
    $totalChecks = $script:SuccessCount + $script:WarningCount + $script:ErrorCount
    Write-Host "Total Checks:   $totalChecks"
    
    if ($script:ErrorCount -eq 0 -and $script:WarningCount -eq 0) {
        Write-Host "`nRESULT: " -NoNewline
        Write-Host "ALL CHECKS PASSED" -ForegroundColor Green -BackgroundColor Black
        Write-Host "The ALOD Testing installation appears to be correctly configured." -ForegroundColor Green
    } elseif ($script:ErrorCount -eq 0) {
        Write-Host "`nRESULT: " -NoNewline
        Write-Host "PASSED WITH WARNINGS" -ForegroundColor Yellow -BackgroundColor Black
        Write-Host "The installation is functional but has warnings that should be reviewed." -ForegroundColor Yellow
    } else {
        Write-Host "`nRESULT: " -NoNewline
        Write-Host "VERIFICATION FAILED" -ForegroundColor Red -BackgroundColor Black
        Write-Host "Critical issues were found. Please review and fix the failed checks." -ForegroundColor Red
    }
}

# Main execution
function Main {
    Write-Host "`n"
    Write-Host "========================================" -ForegroundColor Magenta
    Write-Host "ALOD Testing Installation Verification" -ForegroundColor Magenta
    Write-Host "========================================" -ForegroundColor Magenta
    Write-Host "Configuration: Testing" -ForegroundColor Magenta
    Write-Host "IIS Path: $IISPath" -ForegroundColor Magenta
    Write-Host "Site: $SiteName" -ForegroundColor Magenta
    Write-Host "Application: $AppName" -ForegroundColor Magenta
    Write-Host "Database Server: $($TestingConfig.DatabaseServer)" -ForegroundColor Magenta
    Write-Host "Database: $($TestingConfig.DatabaseName)" -ForegroundColor Magenta
    Write-Host "========================================" -ForegroundColor Magenta
    
    # Check administrator privileges
    if (-not (Test-Administrator)) {
        Write-Warning "This script is not running with Administrator privileges. Some checks may fail."
        Write-Warning "Consider running PowerShell as Administrator for complete verification."
    }
    
    # Initialize modules
    if (-not (Initialize-RequiredModules)) {
        Write-Host "`nCannot proceed without required modules." -ForegroundColor Red
        return 1
    }
    
    # Run verification checks
    Test-IISConfiguration | Out-Null
    Test-ApplicationPool | Out-Null
    Test-WebConfig | Out-Null
    Test-DatabaseConnectivity | Out-Null
    Test-SqlLogin | Out-Null
    Test-DatabaseUser | Out-Null
    Test-SessionStateTables | Out-Null
    
    # Show summary
    Show-Summary
    
    # Return exit code
    if ($script:ErrorCount -eq 0) {
        return 0
    } else {
        return 1
    }
}

# Execute main function
$exitCode = Main
exit $exitCode
