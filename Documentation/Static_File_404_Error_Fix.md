# Static File 404 Error Fix - ALOD Website

**Date:** October 29, 2025  
**Issue Type:** IIS Configuration / Static Resource Loading  
**Status:** ✅ Resolved  

## Problem Description

The ALOD website was experiencing a static file error when trying to load images and other resources:

```
Detailed Error Information:
Module         IIS Web Core
Notification   MapRequestHandler
Handler        StaticFile
Error Code     0x80070002
Requested URL  http://alod-main:80/App_Themes/DefaultBlue/images/ALOD3.jpg
Physical Path  C:\inetpub\wwwroot\App_Themes\DefaultBlue\images\ALOD3.jpg
Logon Method   Anonymous
Logon User     Anonymous
```

**Error Code:** `0x80070002` - "The system cannot find the file specified"

## Root Cause Analysis

### Issue Identification
1. **Incorrect Path Resolution**: The ALOD application was generating URLs for static resources without including the port number (8085)
2. **Default Port Behavior**: Browsers default to port 80 when no port is specified in URLs
3. **Wrong Physical Directory**: Requests were being routed to the Default Web Site's directory (`C:\inetpub\wwwroot`) instead of the ALOD application directory (`C:\inetpub\ALOD`)

### Technical Details
- **Expected Request**: `http://alod-main:8085/App_Themes/DefaultBlue/images/ALOD3.jpg`
- **Actual Request**: `http://alod-main:80/App_Themes/DefaultBlue/images/ALOD3.jpg`
- **Expected Physical Path**: `C:\inetpub\ALOD\App_Themes\DefaultBlue\images\ALOD3.jpg`
- **Actual Physical Path**: `C:\inetpub\wwwroot\App_Themes\DefaultBlue\images\ALOD3.jpg`

## Solution Implementation

### Step 1: Add Port 80 Binding to ALOD Website
```powershell
New-WebBinding -Name "ALOD" -Protocol "http" -Port 80 -HostHeader "alod-main"
```

### Step 2: Stop Default Web Site to Prevent Port Conflicts
```powershell
Stop-Website -Name "Default Web Site"
```

### Step 3: Verify Configuration
```powershell
Get-WebBinding -Name "ALOD" | Select-Object protocol, bindingInformation, hostHeader
```

## Configuration Before Fix

| Website | State | Bindings | App Pool | Physical Path |
|---------|-------|----------|----------|---------------|
| Default Web Site | Started | `*:80:` | DefaultAppPool | %SystemDrive%\inetpub\wwwroot |
| ALOD | Started | `*:8085:alod-main` | ALODAppPool | C:\inetpub\ALOD |
| SRXLite | Started | `*:8090:alod-loc` | SRXLiteAppPool | C:\inetpub\SRXLite |

## Configuration After Fix

| Website | State | Bindings | App Pool | Physical Path |
|---------|-------|----------|----------|---------------|
| **Default Web Site** | ⏹️ Stopped | `*:80:` | N/A | %SystemDrive%\inetpub\wwwroot |
| **ALOD** | ✅ Started | `*:8085:alod-main`<br>`*:80:alod-main` | ALODAppPool | C:\inetpub\ALOD |
| **SRXLite** | ✅ Started | `*:8090:alod-loc` | SRXLiteAppPool | C:\inetpub\SRXLite |

## Verification Results

### URL Testing
- ✅ **ALOD (Port 80)**: `http://alod-main` - HTTP 200 OK
- ✅ **ALOD (Port 8085)**: `http://alod-main:8085` - HTTP 200 OK  
- ✅ **SRXLite (Port 8090)**: `http://alod-loc:8090` - HTTP 200 OK

### Static File Testing
- ✅ **Images**: `http://alod-main/App_Themes/DefaultBlue/images/ALOD3.jpg` - HTTP 200 OK
- ✅ **CSS/JS**: All theme resources now load from correct directory

### PowerShell Verification Commands
```powershell
# Test main application
Invoke-WebRequest -Uri "http://alod-main" -Method GET

# Test specific static file that was failing
Invoke-WebRequest -Uri "http://alod-main/App_Themes/DefaultBlue/images/ALOD3.jpg" -Method GET

# Verify bindings
Get-WebBinding -Name "ALOD"
```

## Benefits of This Solution

### 1. **Dual Port Access**
- Users can access ALOD via both `http://alod-main` (port 80) and `http://alod-main:8085`
- Provides flexibility for different integration scenarios

### 2. **Correct Static Resource Loading**
- All images, CSS, JavaScript, and other static resources load from the correct directory
- No more 404 errors for theme assets

### 3. **No Application Code Changes Required**
- Solution implemented entirely through IIS configuration
- No need to modify ASP.NET application code to fix URL generation

### 4. **Preserved Existing Functionality**
- SRXLite continues to work on port 8090
- ALOD retains its original port 8085 binding for backward compatibility

## Troubleshooting Guide

### If Static Files Still Don't Load
1. **Check File Permissions**: Ensure IIS_IUSRS has read access to `C:\inetpub\ALOD`
2. **Verify Physical Path**: Confirm files exist in the ALOD directory, not wwwroot
3. **Check Application Pool**: Ensure ALODAppPool is running and assigned to ALOD site

### If Port 80 Conflicts Occur
```powershell
# Check what's using port 80
netstat -ano | findstr :80

# Stop conflicting services
Stop-Website -Name "Default Web Site"
```

### If Host Header Issues Arise
```powershell
# Verify hosts file entries
Get-Content C:\Windows\System32\drivers\etc\hosts | Select-String "alod"

# Should show:
# 127.0.0.1    alod-main
# 127.0.0.1    alod-loc
```

## Related Documentation
- [Debug Mode Deployment Guide](VS_Code_Migration_Summary.md)
- [Build Configuration Report](Build_Configuration_Report.md)
- [IIS Application Pool Setup](../README.md#iis-configuration)

## Technical Notes

### IIS Binding Priority
When multiple sites have bindings on the same port, IIS uses this priority order:
1. **Specific Host Header** (e.g., `*:80:alod-main`) - Highest priority
2. **All Unassigned** (e.g., `*:80:`) - Lower priority

This is why adding the specific host header binding to ALOD allows it to handle requests for `alod-main` on port 80, while the Default Web Site (with `*:80:`) would handle other hostnames.

### ASP.NET URL Generation
ASP.NET WebForms applications sometimes generate absolute URLs based on the current request context. When the application detects it's running on a non-standard port, it may include the port in some URLs but not others, leading to inconsistent behavior. The dual-binding approach ensures URLs work correctly regardless of how they're generated.

---

**Resolution Confirmed:** October 29, 2025  
**Tested By:** AI Assistant  
**Environment:** Windows IIS 10, .NET Framework 4.8.1, ALOD/ECT System