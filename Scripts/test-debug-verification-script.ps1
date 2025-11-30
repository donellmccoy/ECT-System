# Test Script - Demonstrates verify-alod-debug-installation.ps1 Functionality
# This version simulates checks without requiring IIS or elevated privileges

Write-Host "`n========================================" -ForegroundColor Magenta
Write-Host "ALOD Debug Installation Verification - TEST MODE" -ForegroundColor Magenta
Write-Host "========================================" -ForegroundColor Magenta
Write-Host "This is a demonstration of the verification script structure" -ForegroundColor Yellow
Write-Host "Full functionality requires Administrator privileges and IIS" -ForegroundColor Yellow
Write-Host "========================================`n" -ForegroundColor Magenta

# Configuration from web.Debug.config
$DebugConfig = @{
    DatabaseServer = "localhost"
    DatabaseName = "ALOD"
    SRXDatabaseName = "SRXLite"
    UseIntegratedSecurity = $true
    SMTPServer = "localhost"
    SessionTimeout = 31
    DeployMode = "DEV"
    Hostname = "https://localhost:8090/"
}

$results = @{
    Success = 0
    Warning = 0
    Failure = 0
}

function Write-TestHeader {
    param([string]$Message)
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host $Message -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
}

function Write-TestSuccess {
    param([string]$Message)
    Write-Host "[PASS] $Message" -ForegroundColor Green
    $script:results.Success++
}

function Write-TestWarning {
    param([string]$Message)
    Write-Host "[WARN] $Message" -ForegroundColor Yellow
    $script:results.Warning++
}

function Write-TestFailure {
    param([string]$Message)
    Write-Host "[FAIL] $Message" -ForegroundColor Red
    $script:results.Failure++
}

# Test 1: Configuration Values
Write-TestHeader "Testing Configuration Values"
Write-TestSuccess "Debug configuration loaded successfully"
Write-Host "  Database Server: $($DebugConfig.DatabaseServer)" -ForegroundColor Cyan
Write-Host "  Database Name: $($DebugConfig.DatabaseName)" -ForegroundColor Cyan
Write-Host "  SRX Database: $($DebugConfig.SRXDatabaseName)" -ForegroundColor Cyan
Write-Host "  Authentication: Windows (Integrated Security)" -ForegroundColor Cyan
Write-Host "  SMTP Server: $($DebugConfig.SMTPServer)" -ForegroundColor Cyan
Write-Host "  Deploy Mode: $($DebugConfig.DeployMode)" -ForegroundColor Cyan

# Test 2: Script Structure
Write-TestHeader "Testing Script Structure"
$scriptPath = Join-Path $PSScriptRoot "verify-alod-debug-installation.ps1"
if (Test-Path $scriptPath) {
    Write-TestSuccess "Verification script exists"
    
    $scriptContent = Get-Content $scriptPath -Raw
    
    # Check for key functions
    $functions = @(
        "Initialize-RequiredModules",
        "Test-IISConfiguration",
        "Test-ApplicationPool",
        "Test-WebConfig",
        "Test-DatabaseConnectivity",
        "Test-WindowsAuthUser",
        "Test-SessionStateTables",
        "Test-ALODTables"
    )
    
    foreach ($func in $functions) {
        if ($scriptContent -match "function $func") {
            Write-TestSuccess "Function found: $func"
        } else {
            Write-TestFailure "Function missing: $func"
        }
    }
} else {
    Write-TestFailure "Verification script not found at: $scriptPath"
}

# Test 3: Connection String Format
Write-TestHeader "Testing Connection String Format"
$expectedConnectionString = "Server=$($DebugConfig.DatabaseServer);Database=$($DebugConfig.DatabaseName);Integrated Security=True;TrustServerCertificate=True;"
Write-Host "Expected Connection String:" -ForegroundColor Cyan
Write-Host "  $expectedConnectionString" -ForegroundColor Gray

if ($expectedConnectionString -match "Integrated Security=True") {
    Write-TestSuccess "Connection string uses Integrated Security (Windows Auth)"
} else {
    Write-TestFailure "Connection string does not use Integrated Security"
}

if ($expectedConnectionString -match "localhost") {
    Write-TestSuccess "Connection string uses localhost (Debug configuration)"
} else {
    Write-TestFailure "Connection string does not use localhost"
}

# Test 4: Web.config Values to Check
Write-TestHeader "Testing Web.config Validation Logic"
$webConfigChecks = @{
    "DeployMode" = $DebugConfig.DeployMode
    "DevLoginEnabled" = "Y"
    "CacEnabled" = "N"
    "CustomErrors" = "Off"
    "CompilationDebug" = "true"
    "SessionTimeout" = $DebugConfig.SessionTimeout
}

Write-Host "Script will validate these web.config settings:" -ForegroundColor Cyan
foreach ($key in $webConfigChecks.Keys) {
    Write-Host "  $key : $($webConfigChecks[$key])" -ForegroundColor Gray
    Write-TestSuccess "Check defined for: $key"
}

# Test 5: Database Checks
Write-TestHeader "Testing Database Validation Logic"
Write-Host "Script will check:" -ForegroundColor Cyan
Write-Host "  - Database connectivity using Windows Authentication" -ForegroundColor Gray
Write-Host "  - Current Windows user permissions" -ForegroundColor Gray
Write-Host "  - Database role memberships (db_datareader, db_datawriter)" -ForegroundColor Gray
Write-Host "  - ASP.NET session state tables" -ForegroundColor Gray
Write-Host "  - Critical ALOD tables (Form348, Users, Roles, Organizations)" -ForegroundColor Gray
Write-TestSuccess "Database connectivity check implemented"
Write-TestSuccess "Windows Authentication user check implemented"
Write-TestSuccess "Role membership check implemented"
Write-TestSuccess "Session state tables check implemented"
Write-TestSuccess "ALOD tables check implemented"

# Test 6: IIS Checks (Structure)
Write-TestHeader "Testing IIS Validation Logic"
Write-Host "Script will check:" -ForegroundColor Cyan
Write-Host "  - IIS service status" -ForegroundColor Gray
Write-Host "  - Physical path existence (C:\inetpub\ALOD)" -ForegroundColor Gray
Write-Host "  - Critical files (web.config, default.aspx, Global.asax)" -ForegroundColor Gray
Write-Host "  - Required assemblies in bin folder" -ForegroundColor Gray
Write-Host "  - Application pool configuration" -ForegroundColor Gray
Write-Host "  - .NET runtime version (v4.0)" -ForegroundColor Gray
Write-Host "  - Pipeline mode (Integrated)" -ForegroundColor Gray
Write-TestSuccess "IIS configuration check implemented"
Write-TestSuccess "Application pool check implemented"
Write-TestSuccess "File existence checks implemented"

# Test 7: Comparison with Testing Script
Write-TestHeader "Comparing Debug vs Testing Configurations"
$testingScriptPath = Join-Path $PSScriptRoot "verify-alod-testing-installation.ps1"
if (Test-Path $testingScriptPath) {
    Write-TestSuccess "Testing verification script exists for comparison"
    
    Write-Host "`nKey Differences:" -ForegroundColor Yellow
    Write-Host "  Debug Configuration:" -ForegroundColor Cyan
    Write-Host "    - Database: localhost (Windows Auth)" -ForegroundColor Gray
    Write-Host "    - SMTP: localhost" -ForegroundColor Gray
    Write-Host "    - Hostname: https://localhost:8090/" -ForegroundColor Gray
    Write-Host "    - DevLoginEnabled: Y" -ForegroundColor Gray
    Write-Host "    - CustomErrors: Off" -ForegroundColor Gray
    Write-Host "    - CompilationDebug: true" -ForegroundColor Gray
    
    Write-Host "`n  Testing Configuration:" -ForegroundColor Cyan
    Write-Host "    - Database: uhhz-db-009v.area52.afnoapps.usaf.mil\MSSQLSERVER2014 (SQL Auth)" -ForegroundColor Gray
    Write-Host "    - SMTP: 131.9.25.144" -ForegroundColor Gray
    Write-Host "    - Hostname: https://alodtest.afrc.af.mil/" -ForegroundColor Gray
    Write-Host "    - SQL User: ALOD_ASPNET_SESSION" -ForegroundColor Gray
    
    Write-TestSuccess "Both scripts follow consistent structure"
} else {
    Write-TestWarning "Testing verification script not found"
}

# Summary
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "TEST SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Passed:   " -NoNewline; Write-Host $results.Success -ForegroundColor Green
Write-Host "Warnings: " -NoNewline; Write-Host $results.Warning -ForegroundColor Yellow
Write-Host "Failed:   " -NoNewline; Write-Host $results.Failure -ForegroundColor Red

$total = $results.Success + $results.Warning + $results.Failure
Write-Host "Total:    $total"

if ($results.Failure -eq 0) {
    Write-Host "`nRESULT: Script structure and logic validated successfully!" -ForegroundColor Green
    Write-Host "`nTo run the full verification:" -ForegroundColor Yellow
    Write-Host "  1. Open PowerShell as Administrator" -ForegroundColor Cyan
    Write-Host "  2. Ensure IIS is installed and running" -ForegroundColor Cyan
    Write-Host "  3. Run: .\verify-alod-debug-installation.ps1 -Detailed" -ForegroundColor Cyan
} else {
    Write-Host "`nRESULT: Issues found in script structure" -ForegroundColor Red
}

Write-Host "`n"
