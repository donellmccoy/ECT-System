# SRXLite - Document Management System

SRXLite is a comprehensive document management and batch processing system designed for handling scanned documents, digital uploads, and workflow automation within the ECTSystem (Electronic Case Tracking System). It provides a robust platform for document digitization, storage, retrieval, and processing.

## Overview

SRXLite serves as the document processing component of the larger ALOD (Air Force Line of Duty) system, enabling efficient handling of military/government documentation workflows. The system supports various document types, batch operations, and integrates seamlessly with existing case management processes.

## Architecture

### Project Structure

```
SRXLite.sln
├── SRXLite/                    # Main web application
│   ├── Default.aspx           # Main entry point
│   ├── Services/              # Web services
│   ├── Controls/              # Custom ASP.NET controls
│   ├── Reports/               # Reporting functionality
│   ├── Handlers/              # HTTP handlers
│   ├── configFiles/           # Configuration files
│   └── Web.config             # Application configuration
├── SRXLite.Database/          # Database schema & scripts
│   ├── dbo/                   # Database objects
│   ├── Scripts/               # SQL scripts
│   └── Security/              # Database security
├── SRXLiteWebUtility/         # Core business logic library
│   ├── Classes/               # Domain classes (Document, Batch, User, etc.)
│   ├── DataAccess/            # Data access layer
│   ├── DataTypes/             # Data transfer objects
│   └── Modules/               # Utility modules
└── BuildScripts/              # Build automation
```

### Technology Stack

- **Framework**: .NET Framework 4.8
- **Frontend**: ASP.NET Web Forms (VB.NET)
- **Backend**: VB.NET class libraries
- **Database**: SQL Server
- **PDF Processing**: ABCpdf v12.5.1
- **UI Controls**: AjaxControlToolkit v20.1.0
- **Reporting**: Microsoft ReportViewer
- **Logging**: log4net v2.0.15, NLog v5.2.4
- **Email**: MailKit v4.2.0
- **Cryptography**: BouncyCastle
- **Dependency Injection**: Microsoft Unity
- **ORM**: Custom data access layer with async operations

## Key Features

### Document Management
- **Multi-format Support**: PDF, images, and various document types
- **Batch Processing**: Bulk document operations and uploads
- **Version Control**: Document versioning and history tracking
- **Digital Signatures**: Integrated signature workflow
- **OCR Integration**: Text extraction from scanned documents

### Security & Access Control
- **Role-based Access**: Granular permissions system
- **Encryption**: Secure document storage and transmission
- **Audit Logging**: Comprehensive activity tracking
- **Authentication**: Forms-based authentication with impersonation

### Workflow Integration
- **ALOD Integration**: Seamless integration with Line of Duty investigations
- **Status Tracking**: Document status and workflow state management
- **Approval Workflows**: Multi-level approval processes
- **Notification System**: Email notifications for workflow events

### Performance & Scalability
- **Asynchronous Processing**: Non-blocking operations for better performance
- **Memory Management**: Efficient memory stream handling
- **Caching**: Strategic caching for improved response times
- **Background Processing**: Long-running tasks handled asynchronously

## Installation & Setup

### Prerequisites

- **Operating System**: Windows Server 2016+ or Windows 10/11
- **Web Server**: IIS 8.5+ with ASP.NET 4.8
- **Database**: SQL Server 2016+ (Express edition supported)
- **Development Tools**: Visual Studio 2019+ (for development)
- **Memory**: Minimum 8GB RAM, 16GB recommended
- **Disk Space**: 50GB+ for document storage

### Database Setup

1. **Create Database**:
   ```sql
   CREATE DATABASE SRXLite;
   ```

2. **Run Schema Scripts**:
   - Execute scripts in `SRXLite.Database/Scripts/` in order
   - Apply permissions from `SRXLite.Database/Permissions.sql`

3. **Configure Connection String**:
   Update `SRXLite/configFiles/connectionStrings.config`:
   ```xml
   <connectionStrings>
     <add name="SRXLiteConnectionString"
          connectionString="Server=YOUR_SERVER;Database=SRXLite;Integrated Security=True"
          providerName="System.Data.SqlClient"/>
   </connectionStrings>
   ```

### Application Configuration

1. **Web.config Settings**:
   - Update `configFiles/appSettings.config` for environment-specific settings
   - Configure SMTP settings for email notifications
   - Set encryption keys and security parameters

2. **IIS Configuration**:
   - Create application pool with .NET Framework 4.8
   - Enable Windows authentication and impersonation
   - Configure SSL certificate for secure access
   - Set up virtual directories for document storage

3. **File System Permissions**:
   - Grant IIS application pool identity read/write access to document storage folders
   - Configure network shares if using distributed storage

### Build & Deployment

1. **Restore NuGet Packages**:
   ```powershell
   nuget restore SRXLite.sln
   ```

2. **Build Solution**:
   ```powershell
   msbuild SRXLite.sln /p:Configuration=Release
   ```

3. **Deploy to IIS**:
   - Copy built files to IIS web root
   - Update configuration files for target environment
   - Restart IIS application pool

## Configuration

### Environment-Specific Settings

SRXLite supports multiple deployment environments:

- **Development**: Local development with debug logging
- **Test**: Staging environment with full logging
- **Production**: Live environment with optimized settings

Configuration files are located in `SubWebConfigs/` directory and are applied during build process.

### Key Configuration Areas

- **Document Storage**: Configure paths and retention policies
- **Batch Processing**: Set limits and processing parameters
- **Security**: Encryption keys, session timeouts, password policies
- **Integration**: ALOD system connection settings
- **Performance**: Caching, connection pooling, thread limits

## Usage

### Basic Operations

1. **Document Upload**:
   - Access upload interface through web application
   - Select document type and associated case/entity
   - Upload single files or batch import

2. **Batch Processing**:
   - Create batch jobs for bulk operations
   - Monitor processing status and results
   - Handle errors and retry failed operations

3. **Document Retrieval**:
   - Search documents by various criteria
   - View documents in browser or download
   - Access version history and metadata

### API Integration

SRXLite provides web services for integration:

- **Document Services**: Upload, download, and manage documents
- **Batch Services**: Create and monitor batch operations
- **Search Services**: Query documents and metadata
- **Workflow Services**: Integration with external workflows

### Administration

- **User Management**: Create and manage user accounts
- **Permission Configuration**: Set up role-based access control
- **System Monitoring**: View logs, performance metrics, and system health
- **Maintenance**: Database cleanup, index optimization, backup management

## Development

### Development Environment Setup

1. **Clone Repository**:
   ```bash
   git clone <repository-url>
   cd ECTSystem
   ```

2. **Open Solution**:
   - Open `AFLOD.sln` in Visual Studio
   - Ensure all projects load correctly

3. **Database Setup**:
   - Create local SQL Server database
   - Run database scripts from `SRXLite.Database/`

4. **Configure Development Settings**:
   - Update connection strings for local database
   - Configure debug settings in Web.config

### Code Organization

- **SRXLite**: Web application layer with ASPX pages and code-behind
- **SRXLiteWebUtility**: Business logic and data access classes
- **SRXLite.Database**: Database schema and migration scripts

### Testing

- **Unit Tests**: Located in `ALOD.Tests/` (shared test suite)
- **Integration Tests**: Database integration tests using NHibernate
- **UI Tests**: Manual testing procedures documented separately

### Contributing

1. **Code Standards**:
   - Follow VB.NET naming conventions
   - Use Option Strict On
   - Implement proper error handling
   - Add XML documentation comments

2. **Version Control**:
   - Use feature branches for development
   - Follow conventional commit messages
   - Code review required for all changes

## Troubleshooting

### Common Issues

1. **Database Connection Errors**:
   - Verify connection string in config files
   - Check SQL Server service status
   - Validate database permissions

2. **File Upload Failures**:
   - Check IIS upload limits in Web.config
   - Verify file system permissions
   - Review available disk space

3. **Performance Issues**:
   - Monitor database query performance
   - Check memory usage and garbage collection
   - Review IIS application pool settings

4. **Integration Problems**:
   - Validate ALOD system connectivity
   - Check shared database access
   - Review service account permissions

### Logging

- **Application Logs**: Configured through log4net/NLog
- **Error Logs**: Automatic error logging with stack traces
- **Audit Logs**: User action and system event tracking
- **Performance Logs**: Request timing and resource usage

### Support

For technical support:
- Check application logs in configured log directory
- Review IIS event logs
- Contact system administrator with error details

## Security Considerations

- **Data Encryption**: Documents encrypted at rest and in transit
- **Access Control**: Role-based permissions with principle of least privilege
- **Input Validation**: All user inputs validated and sanitized
- **Session Management**: Secure session handling with timeouts
- **Audit Trail**: Comprehensive logging of all system activities

## Performance Optimization

- **Database Tuning**: Regular index maintenance and query optimization
- **Caching Strategy**: Implement appropriate caching layers
- **Resource Management**: Proper disposal of database connections and file handles
- **Load Balancing**: Support for web farm deployments
- **Monitoring**: Implement performance monitoring and alerting

## Future Enhancements

- Migration to .NET Core/.NET 6+
- RESTful API implementation
- Cloud storage integration (Azure Blob Storage)
- Advanced OCR and AI-powered document processing
- Mobile application support
- Enhanced reporting and analytics

---

**Version**: 1.0.0
**Last Updated**: September 2025
**Contact**: System Administration Team