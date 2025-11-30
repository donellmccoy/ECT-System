# ALOD Installation Verification Scripts

## Overview

Two PowerShell scripts are provided to verify correct ALOD website installations on IIS for different build configurations:

1. **verify-alod-debug-installation.ps1** - For Debug/Development configuration
2. **verify-alod-testing-installation.ps1** - For Testing configuration

## Prerequisites

### Required

- PowerShell 5.1 or higher
- Administrator privileges (run PowerShell as Administrator)
- IIS (Internet Information Services) installed and running
- WebAdministration PowerShell module (included with IIS)

### Recommended

- SqlServer PowerShell module (for enhanced database checks)
  - Install: `Install-Module -Name SqlServer -Force`

## Quick Start

### Debug Configuration

```powershell
# Run basic verification
.\verify-alod-debug-installation.ps1

# Run with detailed output and CSV report
.\verify-alod-debug-installation.ps1 -Detailed

# Custom IIS path
.\verify-alod-debug-installation.ps1 -IISPath "D:\inetpub\ALOD"
```

### Testing Configuration

```powershell
# Run basic verification
.\verify-alod-testing-installation.ps1

# Run with detailed output and CSV report
.\verify-alod-testing-installation.ps1 -Detailed

# Custom IIS path and site
.\verify-alod-testing-installation.ps1 -IISPath "D:\inetpub\ALOD" -SiteName "ALODTest"
```

## What Each Script Verifies

### Common Checks (Both Scripts)

1. **IIS Configuration**
   - IIS service running
   - Physical path exists
   - Critical files present (web.config, default.aspx, Global.asax)
   - Required assemblies in bin folder (ALOD.Core.dll, ALOD.Data.dll, etc.)
   - IIS site status
   - IIS application configuration

2. **Application Pool**
   - Application pool exists and is started
   - .NET runtime version (v4.0 for .NET Framework 4.8.1)
   - Pipeline mode (Integrated recommended)
   - Identity configuration

3. **Web.config Settings**
   - Connection strings match expected configuration
   - appSettings values (DeployMode, hostname, etc.)
   - SMTP configuration
   - Session state settings
   - Compilation debug mode
   - Custom errors mode

4. **Database Connectivity**
   - Connection to ALOD database succeeds
   - Connection to SRXLite database succeeds
   - SQL Server version information

5. **Session State**
   - ASP.NET session state tables exist (ASPStateTempApplications, ASPStateTempSessions)

### Debug-Specific Checks

- **Windows Authentication**: Verifies current user has database access
- **User Permissions**: Checks Windows user's role memberships and permissions
- **ALOD Tables**: Verifies critical tables (Form348, Users, Roles, Organizations)
- **Local Configuration**: Confirms localhost database and SMTP settings

### Testing-Specific Checks

- **SQL Authentication**: Verifies SQL Server login (ALOD_ASPNET_SESSION)
- **Database User**: Checks database user exists and has correct roles
- **Role Assignments**: Validates db_datareader and db_datawriter roles
- **Object Permissions**: Lists specific object-level permissions
- **Remote Configuration**: Confirms Testing environment server settings

## Configuration Differences

### Debug Configuration (web.Debug.config)

| Setting | Value |
|---------|-------|
| Database Server | localhost |
| Database Authentication | Windows (Integrated Security) |
| ALOD Database | ALOD |
| SRXLite Database | SRXLite |
| SMTP Server | localhost |
| Deploy Mode | DEV |
| Hostname | <https://localhost:8090/> |
| DevLoginEnabled | Y |
| CAC Enabled | N |
| Custom Errors | Off |
| Compilation Debug | true |
| Session Timeout | 31 minutes |

### Testing Configuration (web.Testing.config)

| Setting | Value |
|---------|-------|
| Database Server | uhhz-db-009v.area52.afnoapps.usaf.mil\MSSQLSERVER2014 |
| Database Authentication | SQL Server (ALOD_ASPNET_SESSION) |
| ALOD Database | ALOD |
| SRXLite Database | SRXLite |
| SMTP Server | 131.9.25.144 |
| Deploy Mode | DEV |
| Hostname | <https://alodtest.afrc.af.mil/> |
| DevLoginEnabled | Y |
| CAC Enabled | N |
| Session Timeout | 31 minutes |

## Output Format

### Color-Coded Results

- **[PASS]** (Green) - Check passed successfully
- **[WARN]** (Yellow) - Warning that should be reviewed
- **[FAIL]** (Red) - Critical failure requiring attention
- **[INFO]** (Cyan) - Informational message (when using -Detailed)

### Summary Report

The script provides a summary with counts of:

- Passed checks
- Warnings
- Failed checks
- Total checks

### Overall Result

- **ALL CHECKS PASSED** - Installation is correctly configured
- **PASSED WITH WARNINGS** - Functional but has items to review
- **VERIFICATION FAILED** - Critical issues found, requires fixes

### CSV Report (with -Detailed)

When using the `-Detailed` parameter, a CSV report is generated:

- **Debug**: `ALOD_Debug_Verification_Report.csv`
- **Testing**: `ALOD_Testing_Verification_Report.csv`

The CSV contains all check results with Status, Message, and Details columns.

## Common Issues and Solutions

### "WebAdministration module not found"

**Cause**: IIS is not installed or IIS Management Scripts feature is missing
**Solution**:

```powershell
# Install IIS Management Scripts
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ManagementScriptingTools
```

### "Script is not running with Administrator privileges"

**Cause**: PowerShell not running as Administrator
**Solution**: Right-click PowerShell → "Run as Administrator"

### "Database connection failed"

**Debug Configuration**:

- Ensure SQL Server is running on localhost
- Verify current Windows user has database access
- Check if ALOD database exists

**Testing Configuration**:

- Verify network connectivity to Testing SQL Server
- Confirm SQL login credentials are correct
- Check firewall rules allow SQL Server connection

### "IIS site is not started"

**Solution**:

```powershell
Start-Website -Name "Default Web Site"
```

### "Application pool is not started"

**Solution**:

```powershell
Start-WebAppPool -Name "YourAppPoolName"
```

### "Session state tables not found"

**Solution**: Run aspnet_regsql.exe to create session state tables

```cmd
%windir%\Microsoft.NET\Framework64\v4.0.30319\aspnet_regsql.exe -S localhost -E -ssadd -sstype c -d ALOD
```

## Advanced Usage

### Test Specific Components

You can modify the scripts to test only specific areas by commenting out checks in the `Main` function:

```powershell
# Run only web.config and database checks
$webConfigOk = Test-WebConfig
$dbConnectOk = Test-DatabaseConnectivity
```

### Custom Error Handling

Add try-catch blocks around specific checks if you want to continue on errors:

```powershell
try {
    Test-IISConfiguration
} catch {
    Write-Host "IIS check failed but continuing..." -ForegroundColor Yellow
}
```

### Integration with CI/CD

Use the exit code for automated deployments:

```powershell
$exitCode = & .\verify-alod-debug-installation.ps1
if ($exitCode -ne 0) {
    Write-Error "ALOD verification failed"
    exit 1
}
```

## Script Parameters

### Common Parameters (Both Scripts)

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| IISPath | string | C:\inetpub\ALOD | Physical path to ALOD installation |
| SiteName | string | Default Web Site | IIS site name |
| AppName | string | ALOD | IIS application name |
| Detailed | switch | false | Enable detailed output and CSV report |

## Exit Codes

- **0** - All checks passed (warnings allowed)
- **1** - One or more critical checks failed

## Testing the Scripts

A test script is provided to validate the Debug verification script without requiring IIS:

```powershell
.\test-debug-verification-script.ps1
```

This test script:

- Validates script structure
- Checks all functions are defined
- Verifies configuration values
- Compares Debug vs Testing configurations
- Does not require Administrator privileges or IIS

## Related Files

- `web.Debug.config` - Debug configuration transformations
- `web.Testing.config` - Testing configuration transformations
- `Apply-WebConfigTransform.ps1` - Applies web.config transformations
- `publish ALOD to IIS (Debug)` - VS Code task for Debug deployment
- `publish ALOD to IIS (Testing)` - VS Code task for Testing deployment

## Best Practices

1. **Always run verification after deployment** to catch configuration issues early
2. **Use -Detailed flag** for first-time setups or troubleshooting
3. **Save CSV reports** for compliance and audit trails
4. **Review warnings** - they may indicate configuration drift
5. **Test database connectivity** separately if verification fails
6. **Check IIS logs** if web application doesn't respond after passing verification

## Support

For issues or questions:

1. Check the detailed CSV report for specific failure details
2. Review IIS logs in `C:\inetpub\logs\LogFiles`
3. Check Windows Event Viewer → Application and System logs
4. Verify web.config transformations were applied correctly
5. Consult ALOD documentation in `Documentation/` folder

## Version History

- **v1.0** (November 21, 2025) - Initial release
  - Debug configuration verification
  - Testing configuration verification
  - Comprehensive IIS, database, and configuration checks
