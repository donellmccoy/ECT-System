# IIS Express Setup Guide for ALOD/ECT System

This guide explains how to run both ALOD and SRXLite websites using IIS Express instead of full IIS for development purposes.

## Prerequisites

1. **IIS Express** must be installed (usually comes with Visual Studio)
2. **Solution built** - Run `dotnet build AFLOD.sln` first
3. **Database access** - Ensure SQL Server is running and accessible

## Configuration Files

### 1. IIS Express Configuration
**Location**: `.vs\AFLOD\config\applicationhost.config`

This file contains the IIS Express configuration with:

- **ALOD Site**: Port 44300 (localhost)
- **SRXLite Site**: Port 44301 (localhost)  
- **Application Pools**: ALODAppPool and SRXLiteAppPool (.NET 4.0)

### 2. VS Code Tasks
**Location**: `.vscode\tasks.json`

Available tasks:
- `launch ALOD with IIS Express` - Start ALOD only
- `launch SRXLite with IIS Express` - Start SRXLite only  
- `launch both websites with IIS Express` - Start both sites
- `build and launch both websites` - Build solution then start both sites

## Running the Websites

### Option 1: Using VS Code Tasks (Recommended)

1. **Open Command Palette**: `Ctrl+Shift+P`
2. **Run Task**: Type "Tasks: Run Task"
3. **Select Task**: Choose from:
   - "build and launch both websites" (builds solution first)
   - "launch both websites with IIS Express"
   - "launch ALOD with IIS Express" (single site)
   - "launch SRXLite with IIS Express" (single site)

### Option 2: Using PowerShell Script

```powershell
# Launch both websites
.\launch-iisexpress.ps1

# Launch ALOD only
.\launch-iisexpress.ps1 -ALODOnly

# Launch SRXLite only  
.\launch-iisexpress.ps1 -SRXLiteOnly
```

### Option 3: Direct IIS Express Commands

```cmd
# Launch both sites
"C:\Program Files\IIS Express\iisexpress.exe" /config:".vs\AFLOD\config\applicationhost.config"

# Launch specific site
"C:\Program Files\IIS Express\iisexpress.exe" /config:".vs\AFLOD\config\applicationhost.config" /site:ALOD
```

## Website URLs

When running on IIS Express:

| Website | URL | Description |
|---------|-----|-------------|
| **ALOD** | `http://localhost:44300` | Main ECT application |
| **SRXLite** | `http://localhost:44301` | Document service |
| **SRXLite WSDL** | `http://localhost:44301/DocumentService.asmx` | Web service endpoint |

## Advantages of IIS Express

### 🔧 **Development Benefits**
- **No Admin Rights**: Runs without administrator privileges
- **Per-User**: Each developer has their own configuration
- **Lightweight**: Minimal resource usage compared to full IIS
- **VS Code Integration**: Native support in Visual Studio Code

### 🔒 **Security Benefits**  
- **Localhost Only**: Only accessible from local machine by default
- **No Network Exposure**: Safer for development environments
- **Process Isolation**: Separate from system IIS installation

### 🚀 **Performance Benefits**
- **Fast Startup**: Quick launch and shutdown
- **Hot Reload**: Supports file changes without restart
- **Debug Friendly**: Better integration with debugging tools

## Troubleshooting

### Common Issues

#### Port Conflicts
```
Error: Cannot create a file when that file already exists. (0x800700b7)
```

**Solution**: Stop full IIS or change IIS Express ports:
```powershell
# Stop full IIS websites
Stop-Website -Name "ALOD"
Stop-Website -Name "SRXLite"

# Or change ports in applicationhost.config
```

#### Database Connection Issues
```
Login failed for user 'IIS APPPOOL\ALODAppPool'
```

**Solution**: Database permissions may need adjustment for IIS Express:
```sql
-- Grant access to IIS Express identity
CREATE LOGIN [IIS APPPOOL\ALODAppPool] FROM WINDOWS;
USE ALOD_Test;
CREATE USER [IIS APPPOOL\ALODAppPool] FROM LOGIN [IIS APPPOOL\ALODAppPool];
ALTER ROLE db_owner ADD MEMBER [IIS APPPOOL\ALODAppPool];
```

#### Application Won't Start
1. **Check Build**: Ensure `dotnet build AFLOD.sln` succeeds
2. **Check Config**: Verify paths in `applicationhost.config` are correct
3. **Check Permissions**: Ensure user can access project directories

### Useful Commands

```powershell
# Check if IIS Express is running
Get-Process iisexpress -ErrorAction SilentlyContinue

# Stop all IIS Express processes
Get-Process iisexpress -ErrorAction SilentlyContinue | Stop-Process -Force

# Test website connectivity
Invoke-WebRequest -Uri "http://localhost:44300" -Method GET

# View IIS Express logs (in PowerShell where IIS Express was started)
# Logs appear in the console output
```

## Configuration Details

### Application Pool Settings
```xml
<add name="ALODAppPool" managedRuntimeVersion="v4.0" managedPipelineMode="Integrated" />
<add name="SRXLiteAppPool" managedRuntimeVersion="v4.0" managedPipelineMode="Integrated" />
```

### Site Bindings
```xml
<site name="ALOD" id="2" serverAutoStart="true">
    <bindings>
        <binding protocol="http" bindingInformation="*:44300:localhost" />
    </bindings>
</site>

<site name="SRXLite" id="3" serverAutoStart="true">
    <bindings>
        <binding protocol="http" bindingInformation="*:44301:localhost" />
    </bindings>
</site>
```

## Migration from Full IIS

If you're switching from full IIS to IIS Express:

1. **Stop Full IIS Sites**:
   ```powershell
   Stop-Website -Name "ALOD"
   Stop-Website -Name "SRXLite"
   ```

2. **Update Connection Strings**: May need to adjust for different security context

3. **Update Code References**: Change any hardcoded URLs to use relative paths

4. **Test Thoroughly**: Verify all functionality works with IIS Express

## Production Deployment

⚠️ **Important**: IIS Express is for development only. For production:

1. Use full IIS as documented in previous deployment guides
2. IIS Express should not be used in production environments
3. Performance and security characteristics differ from full IIS

---

**Created**: October 29, 2025  
**Updated**: October 29, 2025  
**Version**: 1.0