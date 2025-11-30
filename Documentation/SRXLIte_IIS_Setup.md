# SRXLite IIS Setup Guide for Local environment

This guide provides step-by-step instructions for hosting the SRXLite application in Internet Information Services (IIS) on Windows.

## Prerequisites

- Windows operating system with administrative privileges
- Visual Studio with the SRXLite project
- SQL Server with appropriate credentials
- IIS installed and configured

## Step 1: Enable IIS on Windows

1. Open Control Panel → Programs and Features → Turn Windows features on or off.
2. Install IIS by checking the following components:
   - Internet Information Services
   - Under Web Management Tools:
     - IIS Management Console
   - Under World Wide Web Services:
     - Application Development Features:
       - .NET Extensibility
       - ASP.NET
       - ISAPI Extensions
       - ISAPI Filters
     - Security:
       - Request Filtering
     - Common HTTP Features:
       - Default Document
       - Static Content

## Step 2: Modify the Project Configuration

### Update Connection Strings

1. Open `SRXLite\configFiles\connectionStrings.config`
2. Update the server and credentials for local SQL Server:

```
Server: localhost
User ID/Password: [Local SQL Server connection credentials]
```

### Update Project Properties

1. In Visual Studio, right-click the SRXLite project → Properties
2. Navigate to Web → Servers
3. Change the Project Url from `http://alod-loc/SRXLite` to `http://alod-loc:8090`

## Step 3: Publish the Project

1. In Visual Studio, right-click the SRXLite project → Publish
2. Choose "Folder" as the target and set the path to `C:\inetpub\SRXLite`
3. Configure the publish profile to exclude certain files by adding the following to `FolderProfile.pubxml`:

```xml
<ExcludeFoldersFromDeployment>_UpgradeReport_Files</ExcludeFoldersFromDeployment>
<ExcludeFilesFromDeployment>_UpgradeReport_Files</ExcludeFilesFromDeployment>
<ExcludeFilesFromDeployment>
_UpgradeReport_Files\*;
Bin\System.Web.DataVisualization.Design.dll;
Bin\System.Web.Datavisualization.dll
</ExcludeFilesFromDeployment>
<ExcludeFilesFromDeployment>$(ExcludeFilesFromDeployment);_UpgradeReport_Files\*.*;</ExcludeFilesFromDeployment>
```

4. Click Publish to generate the compiled files
5. Copy the `SRXLite\configFiles` directory to the publish directory (`C:\inetpub\SRXLite`)

## Step 4: Create IIS Site

1. Open IIS Manager (press Start, type `inetmgr`)
2. In the Connections pane → Sites → Right-click → Add Website
3. Configure the site:
   - Site name: `alod-loc`
   - Physical path: `C:\inetpub\SRXLite` (the publish folder)
   - Binding:
     - Type: `http`
     - Host name: `alod-loc`
     - Port: `8090`
4. Click OK

## Step 5: Configure Application Pool

1. In IIS Manager, navigate to Application Pools
2. Locate the application pool for SRXLite
3. Ensure the following settings:
   - .NET CLR Version: Matches your project (for ASP.NET Core, set to "No Managed Code")
   - Process Model → Identity: Appropriate service account
   - Ensure the pool is Started and in Integrated mode

## Step 6: Update Hosts File

Since the project URL uses `http://alod-loc/`, Windows must resolve this hostname locally.

Run the following PowerShell commands as Administrator:

```powershell
Add-Content -Path "$env:SystemRoot\System32\drivers\etc\hosts" -Value "127.0.0.1 alod-loc"
Ipconfig /flushdns
```

## Step 7: Update SRXLite Bridge

### Update SRXDocumentStore.cs

1. Locate the `SRXDocumentStore.cs` file
2. Update the DocService URL:

```csharp
DocService.Url = "http://alod-loc:8090/services/documentService.asmx"
```

### Regenerate SRXExchange Web Reference

The SRXExchange web reference serves as a bridge between ALOD and the SRXLite service.

1. In the ALOD.Data project, delete the existing SRExchange (or SRXExchange) web reference
2. Right-click the project → Add → Service References → Advanced → Add Web References
3. Enter the URL: `http://alod-loc:8090/services/documentservice.asmx`
4. Click "Go"
5. Set Web reference name to: `SRXExchange`
6. Click "Add Reference"

**Note:** The original reference was named SRExchange. The standardized name SRXExchange improves clarity.

## Step 8: Test the Setup

### Test 1: Verify SRXLite Service

1. Open a web browser and navigate to: `http://alod-loc:8090/Services/DocumentService.asmx`
2. Verify the service loads correctly. You should see the service page:

![SRXLite Service Page](Screenshots/SRXLite_Service.png)

*Figure 1: SRXLite Document Service main page*

3. Click on the service link to view the WSDL:

![SRXLite Service WSDL](Screenshots/SRXLite_Service_WSDL.png)

*Figure 2: SRXLite Document Service WSDL definition*

### Test 2: Verify ALOD Integration

1. Run the ALOD application
2. Attempt to "start a new LOD" to confirm integration works. You should see the LOD Medical page:

![ALOD LOD Medical Page](Screenshots/ALOD_LOD_Medical(Next to start-new-load).png)

*Figure 3: ALOD LOD Medical page showing successful integration*