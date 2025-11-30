<#
.SYNOPSIS
    Verifies the correct installation of the ALOD website in Debug configuration on IIS.

.DESCRIPTION
    This script performs comprehensive verification of the ALOD website deployment to IIS
    in the Debug configuration. It validates 46+ critical checks across multiple categories:
    
    - IIS Configuration: Service status, site existence, physical path, critical files
    - Application Pool: State, .NET version (v4.0), pipeline mode, identity settings
    - Web.config Settings: Connection strings, Debug-specific app settings, SMTP, session state
    - Database Connectivity: Windows Authentication to ALOD and SRXLite databases
    - User Permissions: Database role memberships and access rights
    - Session State: ASP.NET session state tables (ASPStateTempApplications, ASPStateTempSessions)
    - Database Schema: Verification of critical ALOD tables
    
    The script validates against settings defined in web.Debug.config to ensure proper
    deployment and configuration for the local Debug/Development environment with:
    - localhost SQL Server instance
    - Windows Integrated Security authentication
    - Debug compilation mode enabled
    - Custom errors disabled for detailed error messages
    - Development login enabled (DevLoginEnabled=Y)
    - Local SMTP server (localhost)

.PARAMETER IISPath
    The IIS physical path where ALOD is deployed. Defaults to C:\inetpub\ALOD

.PARAMETER SiteName
    The IIS site name. Defaults to "Default Web Site"

.PARAMETER AppName
    The IIS application name. Defaults to "ALOD"

.PARAMETER Detailed
    If specified, provides detailed output for all checks including informational messages

.EXAMPLE
    .\verify-alod-debug-installation.ps1
    Runs basic verification checks with default settings. Shows pass/fail/warning status
    for all checks with summary statistics.

.EXAMPLE
    .\verify-alod-debug-installation.ps1 -Detailed
    Runs verification with detailed output including INFO messages for all configuration
    values detected (SQL Server version, connection strings, app settings, etc.).

.EXAMPLE
    .\verify-alod-debug-installation.ps1 -IISPath "D:\inetpub\ALOD" -SiteName "ALODDebug"
    Runs verification with custom IIS path and site name for non-standard deployments.

.OUTPUTS
    Exit Code 0: All checks passed (or passed with warnings only)
    Exit Code 1: One or more critical checks failed

.NOTES
    Author: ALOD Development Team
    Date: November 21, 2025
    Version: 1.0
    Requires: PowerShell 5.1+, IIS with WebAdministration module, SqlServer module
    Configuration: Debug/Development environment (web.Debug.config)
    
    Prerequisites:
    - IIS must be installed and W3SVC service running
    - WebAdministration module (built-in with IIS)
    - SqlServer PowerShell module (install via: Install-Module SqlServer)
    - ALOD website deployed to IIS physical path
    - SQL Server accessible on localhost with Windows Authentication
    - Current Windows user must have database access (db_datareader, db_datawriter or db_owner)
    
    Known Warnings (Expected):
    - User may be missing db_datareader/db_datawriter if user has db_owner role (db_owner includes these)
    - ALOD tables not found in ALOD schema if tables are in dbo schema instead (common configuration)
    
    Administrator Privileges:
    - Script can run without elevation but some IIS provider checks work best with admin rights
    - If not elevated, a warning is displayed but most checks (46+) will still execute successfully
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

# Debug configuration values from web.Debug.config
$DebugConfig = @{
    DatabaseServer = "localhost"
    DatabaseName = "ALOD"
    SRXDatabaseName = "SRXLite"
    UseIntegratedSecurity = $true  # Debug uses Windows Authentication
    SMTPServer = "localhost"
    SMTPPort = 25
    SessionTimeout = 31
    ExpectedAppPoolIdentity = "ApplicationPoolIdentity"
    DeployMode = "DEV"
    Hostname = "https://localhost:8090/"
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

# Verify web.config and Debug configuration
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
                
                # Verify it uses localhost
                if ($lodConnection.connectionString -like "*$($DebugConfig.DatabaseServer)*") {
                    Write-Success "LOD connection string uses correct Debug server (localhost)"
                } else {
                    Write-Failure "LOD connection string does not use localhost. Expected: $($DebugConfig.DatabaseServer)"
                    Write-Info "Actual: $($lodConnection.connectionString)"
                }
                
                # Verify database name
                if ($lodConnection.connectionString -like "*$($DebugConfig.DatabaseName)*") {
                    Write-Success "LOD connection string uses correct database name"
                } else {
                    Write-Warning "LOD connection string may not use correct database name. Expected: $($DebugConfig.DatabaseName)"
                }
                
                # Verify Integrated Security for Debug
                if ($lodConnection.connectionString -like "*Integrated Security*") {
                    Write-Success "LOD connection string uses Integrated Security (Windows Authentication)"
                } else {
                    Write-Warning "LOD connection string does not use Integrated Security. Debug config typically uses Windows Authentication."
                }
            } else {
                Write-Failure "LOD connection string not found in web.config"
            }
            
            # Check SRXLite connection string
            $srxConnection = $connectionStrings.add | Where-Object { $_.name -eq "SRXLite" }
            if ($null -ne $srxConnection) {
                Write-Success "SRXLite connection string found"
                
                if ($srxConnection.connectionString -like "*$($DebugConfig.SRXDatabaseName)*") {
                    Write-Success "SRXLite connection string uses correct database name"
                } else {
                    Write-Warning "SRXLite connection string may not use correct database name. Expected: $($DebugConfig.SRXDatabaseName)"
                }
            } else {
                Write-Warning "SRXLite connection string not found in web.config"
            }
        } else {
            Write-Failure "connectionStrings section not found in web.config"
        }
        
        # Check appSettings for Debug-specific values
        $appSettings = $webConfig.configuration.appSettings
        if ($null -ne $appSettings) {
            Write-Success "appSettings section exists"
            
            # Check DeployMode
            $deployMode = ($appSettings.add | Where-Object { $_.key -eq "DeployMode" }).value
            if ($deployMode -eq $DebugConfig.DeployMode) {
                Write-Success "DeployMode is set to $($DebugConfig.DeployMode) (Debug configuration)"
            } else {
                Write-Warning "DeployMode is '$deployMode'. Expected '$($DebugConfig.DeployMode)' for Debug."
            }
            
            # Check DevLoginEnabled
            $devLoginEnabled = ($appSettings.add | Where-Object { $_.key -eq "DevLoginEnabled" }).value
            if ($devLoginEnabled -eq "Y") {
                Write-Success "DevLoginEnabled is set to Y (Development mode)"
            } else {
                Write-Info "DevLoginEnabled: $devLoginEnabled"
            }
            
            # Check hostname
            $hostname = ($appSettings.add | Where-Object { $_.key -eq "hostname" }).value
            if ($hostname -like "*localhost*") {
                Write-Success "hostname is set to local URL for Debug"
            } else {
                Write-Warning "hostname may not be configured for Debug: $hostname"
            }
            
            # Check CacEnabled
            $cacEnabled = ($appSettings.add | Where-Object { $_.key -eq "CacEnabled" }).value
            if ($cacEnabled -eq "N") {
                Write-Success "CacEnabled is disabled (Debug configuration)"
            } else {
                Write-Info "CacEnabled: $cacEnabled"
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
            if ($smtpHost -eq $DebugConfig.SMTPServer) {
                Write-Success "SMTP server is configured for Debug (localhost)"
            } else {
                Write-Warning "SMTP server is '$smtpHost'. Expected '$($DebugConfig.SMTPServer)' for Debug."
            }
        }
        
        # Check session state configuration
        $sessionState = $webConfig.configuration.'system.web'.sessionState
        if ($null -ne $sessionState) {
            Write-Info "Session State Mode: $($sessionState.mode)"
            Write-Info "Session Timeout: $($sessionState.timeout) minutes"
            
            if ($sessionState.mode -eq "SQLServer") {
                Write-Success "Session state is configured for SQL Server"
                
                if ($sessionState.timeout -eq $DebugConfig.SessionTimeout.ToString()) {
                    Write-Success "Session timeout is configured correctly ($($DebugConfig.SessionTimeout) minutes)"
                } else {
                    Write-Warning "Session timeout is $($sessionState.timeout) minutes. Expected $($DebugConfig.SessionTimeout) minutes for Debug."
                }
            } else {
                Write-Info "Session state mode is $($sessionState.mode)"
            }
        }
        
        # Check compilation debug mode
        $compilation = $webConfig.configuration.'system.web'.compilation
        if ($null -ne $compilation) {
            if ($compilation.debug -eq "true") {
                Write-Success "Compilation debug mode is enabled (Debug configuration)"
            } else {
                Write-Warning "Compilation debug mode is disabled. Should be 'true' for Debug configuration."
            }
        }
        
        # Check customErrors
        $customErrors = $webConfig.configuration.'system.web'.customErrors
        if ($null -ne $customErrors) {
            if ($customErrors.mode -eq "Off") {
                Write-Success "Custom errors are disabled (Debug configuration shows detailed errors)"
            } else {
                Write-Warning "Custom errors mode is '$($customErrors.mode)'. Expected 'Off' for Debug."
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
    
    # Build connection string with Integrated Security
    $connectionString = "Server=$($DebugConfig.DatabaseServer);Database=$($DebugConfig.DatabaseName);Integrated Security=True;TrustServerCertificate=True;"
    
    try {
        Write-Info "Attempting to connect to: $($DebugConfig.DatabaseServer)"
        Write-Info "Database: $($DebugConfig.DatabaseName)"
        Write-Info "Authentication: Windows (Integrated Security)"
        Write-Info "Current User: $env:USERDOMAIN\$env:USERNAME"
        
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $connection.Open()
        
        if ($connection.State -eq "Open") {
            Write-Success "Successfully connected to ALOD database using Windows Authentication"
            
            # Get SQL Server version
            $command = $connection.CreateCommand()
            $command.CommandText = "SELECT @@VERSION AS Version"
            $reader = $command.ExecuteReader()
            if ($reader.Read()) {
                $version = $reader["Version"]
                Write-Info "SQL Server Version: $version"
            }
            $reader.Close()
            
            # Get current database user
            $command.CommandText = "SELECT SYSTEM_USER AS LoginName, USER_NAME() AS DatabaseUser"
            $reader = $command.ExecuteReader()
            if ($reader.Read()) {
                $loginName = $reader["LoginName"]
                $dbUser = $reader["DatabaseUser"]
                Write-Info "SQL Login: $loginName"
                Write-Info "Database User: $dbUser"
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
        $srxConnectionString = "Server=$($DebugConfig.DatabaseServer);Database=$($DebugConfig.SRXDatabaseName);Integrated Security=True;TrustServerCertificate=True;"
        
        $connection = New-Object System.Data.SqlClient.SqlConnection($srxConnectionString)
        $connection.Open()
        
        if ($connection.State -eq "Open") {
            Write-Success "Successfully connected to SRXLite database using Windows Authentication"
            $connection.Close()
        } else {
            Write-Warning "Failed to connect to SRXLite database. Connection state: $($connection.State)"
        }
    } catch {
        Write-Warning "SRXLite database connection failed: $($_.Exception.Message)"
    }
    
    return $true
}

# Verify Windows Authentication user permissions
function Test-WindowsAuthUser {
    Write-CheckHeader "Verifying Windows Authentication User Permissions"
    
    $connectionString = "Server=$($DebugConfig.DatabaseServer);Database=$($DebugConfig.DatabaseName);Integrated Security=True;TrustServerCertificate=True;"
    
    try {
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $connection.Open()
        
        # Get current Windows login
        $command = $connection.CreateCommand()
        $command.CommandText = "SELECT SYSTEM_USER AS LoginName, USER_NAME() AS DatabaseUser"
        $reader = $command.ExecuteReader()
        
        $loginName = ""
        $dbUser = ""
        if ($reader.Read()) {
            $loginName = $reader["LoginName"]
            $dbUser = $reader["DatabaseUser"]
        }
        $reader.Close()
        
        Write-Success "Current Windows user has database access"
        Write-Info "Windows Login: $loginName"
        Write-Info "Database User: $dbUser"
        
        # Check database role memberships for current user
        $query = @"
SELECT 
    dp.name AS DatabaseRole
FROM sys.database_role_members drm
INNER JOIN sys.database_principals dp ON drm.role_principal_id = dp.principal_id
INNER JOIN sys.database_principals dp2 ON drm.member_principal_id = dp2.principal_id
WHERE dp2.name = USER_NAME()
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
            Write-Success "Current user has role memberships:"
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
            Write-Warning "Current user has no explicit role memberships. May rely on group permissions or public role."
        }
        
        # Test actual permissions by attempting a simple query
        $command.CommandText = "SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES"
        try {
            $reader = $command.ExecuteReader()
            if ($reader.Read()) {
                Write-Success "User has read access to database tables"
            }
            $reader.Close()
        } catch {
            Write-Failure "User does not have read access to database tables"
        }
        
        $connection.Close()
    } catch {
        Write-Failure "Error checking Windows Authentication user: $($_.Exception.Message)" $_.Exception.StackTrace
        return $false
    }
    
    return $true
}

# Verify session state database tables
function Test-SessionStateTables {
    Write-CheckHeader "Verifying ASP.NET Session State Tables"
    
    $connectionString = "Server=$($DebugConfig.DatabaseServer);Database=$($DebugConfig.DatabaseName);Integrated Security=True;TrustServerCertificate=True;"
    
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

# Verify critical ALOD database tables
function Test-ALODTables {
    Write-CheckHeader "Verifying Critical ALOD Database Tables"
    
    $connectionString = "Server=$($DebugConfig.DatabaseServer);Database=$($DebugConfig.DatabaseName);Integrated Security=True;TrustServerCertificate=True;"
    
    try {
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $connection.Open()
        
        # Check for critical ALOD tables
        $criticalTables = @(
            "Form348",
            "Users",
            "Roles",
            "Organizations"
        )
        
        $query = @"
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'ALOD'
    AND TABLE_NAME IN ('Form348', 'Users', 'Roles', 'Organizations')
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
        
        foreach ($table in $criticalTables) {
            if ($foundTables -contains $table) {
                Write-Success "Critical ALOD table exists: ALOD.$table"
            } else {
                Write-Warning "Critical ALOD table not found: ALOD.$table"
            }
        }
        
        $connection.Close()
    } catch {
        Write-Warning "Error checking ALOD tables: $($_.Exception.Message)"
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
        Write-Host "The ALOD Debug installation appears to be correctly configured." -ForegroundColor Green
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
    Write-Host "ALOD Debug Installation Verification" -ForegroundColor Magenta
    Write-Host "========================================" -ForegroundColor Magenta
    Write-Host "Configuration: Debug/Development" -ForegroundColor Magenta
    Write-Host "IIS Path: $IISPath" -ForegroundColor Magenta
    Write-Host "Site: $SiteName" -ForegroundColor Magenta
    Write-Host "Application: $AppName" -ForegroundColor Magenta
    Write-Host "Database Server: $($DebugConfig.DatabaseServer)" -ForegroundColor Magenta
    Write-Host "Database: $($DebugConfig.DatabaseName)" -ForegroundColor Magenta
    Write-Host "Authentication: Windows (Integrated Security)" -ForegroundColor Magenta
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
    $iisOk = Test-IISConfiguration
    $appPoolOk = Test-ApplicationPool
    $webConfigOk = Test-WebConfig
    $dbConnectOk = Test-DatabaseConnectivity
    
    if ($dbConnectOk) {
        $winAuthOk = Test-WindowsAuthUser
        $sessionStateOk = Test-SessionStateTables
        $alodTablesOk = Test-ALODTables
    }
    
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
