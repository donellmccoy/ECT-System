# PAL Uploads Project Overview

## Purpose
The **PAL Uploads** project is a batch file processing utility that imports PAL (Physical Activity Log) documents into the ALOD document management system. It automates the ingestion of historical and ongoing PAL documents into the SRXLite document repository.

## What It Does

The application processes PAL documents by:
1. Scanning a configured folder for PAL PDF files
2. Uploading them to the document service (SRXLite) via SOAP web service
3. Creating lookup records in the database for search/retrieval
4. Moving successfully processed files to a success folder

## File Naming Convention

Files must follow this strict format: **`YYYY_MM_LASTNAME_####.pdf`**

- `YYYY` = Year (4 digits)
- `MM` = Month (01-12)
- `LASTNAME` = Service member's last name
- `####` = Last 4 digits of SSN

**Example**: `2023_03_SMITH_1234.pdf`

Files that don't match this format are rejected and marked as failures.

## Processing Flow

### Step-by-Step Process

1. **File Discovery**
   - Reads files from `TopFolder` (configured in `App.config`)
   - Validates filename format (splits on underscore, expects 4 parts)

2. **Filename Parsing**
   - Extracts: Year, Month, Last Name, Last 4 SSN digits
   - If parsing fails, file is skipped

3. **Document Upload**
   - Authenticates with DocumentService using configured credentials
   - Creates document group "PALDocuments"
   - Uploads PDF with metadata:
     - DocTypeID = 72 (PAL Document type)
     - DocStatus = Approved
     - DocDate = File's LastWriteTime
     - DocDescription = Original filename
   - Receives back: Document ID and viewer URL

4. **Database Insert**
   - Inserts lookup record into `PALDocumentLookup` table:
     ```sql
     INSERT INTO PALDocumentLookup 
       (PalDocID, URL, LastName, Last4SSN, DocYear, DocMonth)
     VALUES (@DocID, @URL, @LastName, @Last4SSN, @Year, @Month)
     ```

5. **File Management**
   - Successfully processed files → moved to `SuccessFolder`
   - Failed files → remain in place (or moved to error folder)
   - Console output shows success/failure status

## Architecture

### Dependencies
- **SOAP Web Service**: `DocService` (SRXLite document storage)
- **Database**: SQL Server (`LODConnectionString`)
- **File System**: Reads from configured folders

### Key Classes
- `Program.cs` - Entry point, instantiates and runs Importer
- `Importer.cs` - Main processing logic
  - `Go()` - Initiates directory walk
  - `WalkDirectoryTree_v2()` - Current implementation (filename-based parsing)
  - `WalkDirectoryTree()` - Legacy implementation (folder-based parsing - not used)
  - `MoveFile()` - Moves files to success folder

### Configuration (App.config)
```xml
<appSettings>
  <add key="TopFolder" value="[path to scan]" />
  <add key="SuccessFolder" value="[path for processed files]" />
  <add key="DocServiceUsername" value="[username]" />
  <add key="DocServicePassword" value="[password]" />
  <add key="EntityName" value="[entity name]" />
</appSettings>
<connectionStrings>
  <add name="LODConnectionString" connectionString="[SQL connection]" />
</connectionStrings>
```

## Database Schema

### PALDocumentLookup Table
| Column | Type | Description |
|--------|------|-------------|
| PalDocID | bigint | Document ID from SRXLite |
| URL | string | Document viewer URL |
| LastName | string | Service member's last name |
| Last4SSN | string | Last 4 digits of SSN |
| DocYear | int | Year from filename |
| DocMonth | int | Month from filename |

## Legacy Code

The project contains two processing methods:

1. **`WalkDirectoryTree()`** - **OLD/UNUSED**
   - Parsed folder structure (e.g., `/2023/03/` directories)
   - Expected filename: `LASTNAME_####.pdf`
   - Had special handling for "AR" (archive) folders

2. **`WalkDirectoryTree_v2()`** - **CURRENT**
   - Parses filename only: `YYYY_MM_LASTNAME_####.pdf`
   - No folder structure requirements
   - Cleaner, more flexible implementation

## Usage

### Running the Application
```bash
cd "PAL Uploads"
.\bin\Debug\PAL Uploads.exe
```

The application:
- Runs as a console application
- Processes all files in `TopFolder`
- Outputs success/failure messages to console
- Does NOT recursively scan subdirectories (flat folder structure)

### Error Handling
- Catches exceptions per file (one failure doesn't stop batch)
- Logs errors to console
- Moves failed files to prevent reprocessing
- Maintains error log collection (`log` variable)

## Current Status

- ✅ **Active** project in AFLOD.sln
- ✅ Builds successfully with .NET Framework 4.8.1
- ✅ Uses NuGet packages for dependencies
- ✅ Referenced by solution as a standard C# console application

## Operational Notes

### This is a recurring utility
Unlike the `Snapshot` project (which was one-time), PAL Uploads appears to be run **periodically** to import batches of PAL documents as they become available.

### Typical Use Cases
- Monthly batch imports of PAL documents
- Historical data migration (one-time large batch)
- Ad-hoc uploads as needed

### Prerequisites
1. SRXLite document service must be running and accessible
2. Database connection must be configured
3. TopFolder and SuccessFolder must exist and be writable
4. Files must follow naming convention exactly

## Web Service Integration

### DocumentService SOAP API
Located in `Web References/DocService/`, the application uses:

- `CreateGroup()` - Creates document group
- `UploadDocument()` - Uploads binary file with metadata
- `GetDocumentViewerUrl()` - Gets URL for viewing document
- `ServiceLogin` - Authentication credentials

### Service Configuration
The `notes.txt` file documents required configuration changes:
- `Settings.Designer.cs`
- `Settings.setting`
- `documentservice.disco`
- `documentservice.wsdl`
- `Reference.map`
- `App.config`

All references must point to the correct SRXLite service endpoint.

## Maintenance Considerations

### If Document Service URL Changes
Must update multiple files (see `notes.txt`):
1. Web service references
2. App.config endpoint
3. Settings files

### If Database Schema Changes
Update SQL INSERT statement in `WalkDirectoryTree_v2()`

### If Filename Format Changes
Modify parsing logic in `WalkDirectoryTree_v2()`:
```csharp
List<string> data = fi.Name.Split('_').ToList();
```

## Future Improvements (Not Implemented)

- Recursive subdirectory scanning
- Configurable filename patterns
- Better error logging (file-based logs)
- Dry-run/preview mode
- Duplicate detection
- Parallel processing for large batches
- Resume capability for interrupted batches
