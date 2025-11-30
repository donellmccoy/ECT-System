# ALOD SRXDocument Setup for Test

## Overview
This document outlines the modifications made to `SRXDocumentStore.cs` to enable secure connections to the remote SRXLite service over HTTPS for testing purposes. These changes address SSL/TLS errors encountered when connecting to "https://alod.afrc.af.mil/srxlite/services/documentservice.asmx".

**Note:** These setups are intended for development and testing only. Do not use in production without proper security measures.

## Required Changes in SRXDocumentStore.cs

### 1. Add Using Directive
Add the following at the top of the file (after existing using statements):

```csharp
// Add System.Net for ServicePointManager to handle TLS protocols
using System.Net;
```

### 2. Modify InitWebService Method
Update the `InitWebService` method to include TLS protocol support and a temporary certificate validation bypass:

```csharp
private void InitWebService(string userName)
{
    DocService = new SRXExchange.DocumentService();

    // Enable TLS 1.2, 1.1, and 1.0 to ensure compatibility with HTTPS servers
    // This addresses "Could not create SSL/TLS secure channel" errors
    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

    // Temporary: Bypass certificate validation for development/testing
    // WARNING: This is insecure and should ONLY be used in non-production environments.
    // Remove or replace with proper certificate trust handling in production.
    ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

    // ... existing code ...
}
```

## Configuration Notes
- Ensure the service URL in your config (e.g., `ALOD.Data.Properties.Settings.config`) points to the remote HTTPS endpoint:
  ```
  <value>https://alod.afrc.af.mil/srxlite/services/documentservice.asmx</value>
  ```
- For local testing, you can uncomment and adjust the URL to a local HTTP endpoint if needed.

## Warnings
- The certificate bypass is a security risk and should be removed once the trust issue is resolved (e.g., by installing the server's root certificate).
- Test thoroughly after changes, and rebuild the project.

## Future Additions
[Add more sections here as needed for additional setup steps or troubleshooting.]
