# Azure Pipeline Improvement Recommendations

**Date:** November 13, 2025  
**Pipeline:** azure-pipelines.yml (Production Build for ALOD)  
**Status:** Recommended enhancements for future implementation

---

## 1. Security - Remove Hardcoded Credentials

### Issue
The `web.Production.config` file contains hardcoded passwords:
- Database password: `alodpwd!2009`
- Document service password: `DDRSalod121511*(`

### Recommendation
Add a token replacement step to inject secrets from Azure Key Vault at deployment time.

```yaml
# After FileTransform, before Remove Transform Files
- task: replacetokens@5
  displayName: Replace Tokens with Key Vault Secrets
  inputs:
    targetFiles: '$(Build.ArtifactStagingDirectory)\ALOD\web.config'
    tokenPattern: 'custom'
    tokenPrefix: '__'
    tokenSuffix: '__'
```

### Implementation Steps
1. Store secrets in Azure Key Vault
2. Update `web.Production.config` to use tokens (e.g., `__LOD_DB_PASSWORD__`)
3. Configure pipeline service connection to Key Vault
4. Add token replacement task with Key Vault variable group

---

## 2. Add Build Validation

### Purpose
Verify that all required assemblies are compiled successfully before proceeding to packaging.

### Recommendation
Insert validation step after "Build Solution":

```yaml
- task: PowerShell@2
  displayName: Verify Build Outputs
  inputs:
    targetType: inline
    script: |
      $requiredDlls = @('ALOD.dll', 'ALOD.Core.dll', 'ALOD.Data.dll')
      $binPath = "$(Build.SourcesDirectory)\ALOD\bin"
      $missing = $requiredDlls | Where-Object { -not (Test-Path "$binPath\$_") }
      if ($missing) {
        throw "Missing required assemblies: $($missing -join ', ')"
      }
      Write-Host "All required assemblies present"
```

### Benefits
- Early detection of build failures
- Prevents publishing incomplete artifacts
- Clear error messages for troubleshooting

---

## 3. Artifact Size Reporting

### Purpose
Track artifact size trends over time to identify bloat and optimize deployment packages.

### Recommendation
Add before "Pipeline Summary":

```yaml
- task: PowerShell@2
  displayName: Report Artifact Size
  inputs:
    targetType: inline
    script: |
      $zipFile = "$(Build.ArtifactStagingDirectory)/$(artifactName).zip"
      $sizeMB = [Math]::Round((Get-Item $zipFile).Length / 1MB, 2)
      Write-Host "##vso[task.setvariable variable=ArtifactSizeMB]$sizeMB"
      Write-Host "Artifact size: $sizeMB MB"
```

### Benefits
- Visibility into artifact size changes
- Variable can be used in pipeline summary or notifications
- Helps identify when to optimize artifact contents

---

## 4. Improve Artifact Naming Documentation

### Current Issue
The `artifactName` variable uses `.v` prefix (`ALOD.Production.v20251113.32.zip`), but documentation shows different format.

### Recommendation
Update header documentation to match actual naming:

```yaml
#   - ZIP File Name: "ALOD.<Configuration>.v<BuildNumber>.zip"
#     Example: ALOD.Production.v20251113.32.zip
```

### Alternative
Remove `.v` prefix from `artifactName` variable if not needed:

```yaml
artifactName: 'ALOD.${{ parameters.buildConfiguration }}.$(Build.BuildNumber)'
```

---

## 5. Add Deployment Readiness Check

### Purpose
Validate that critical configuration sections exist after XDT transformation to prevent runtime failures.

### Recommendation
Add after "Transform Web.Config":

```yaml
- task: PowerShell@2
  displayName: Validate Transformed Config
  inputs:
    targetType: inline
    script: |
      [xml]$config = Get-Content "$(Build.ArtifactStagingDirectory)\ALOD\web.config"
      
      # Verify LOD connection string
      $connString = $config.configuration.connectionStrings.add | Where-Object { $_.name -eq 'LOD' }
      if (-not $connString) {
        throw "LOD connection string missing after transformation"
      }
      
      # Verify critical app settings
      $requiredSettings = @('DeployMode', 'EmailEnabled', 'CacEnabled')
      $appSettings = $config.configuration.appSettings.add
      $missingSettings = $requiredSettings | Where-Object { 
        -not ($appSettings | Where-Object { $_.key -eq $_ }) 
      }
      
      if ($missingSettings) {
        throw "Missing required app settings: $($missingSettings -join ', ')"
      }
      
      Write-Host "Configuration validation passed"
```

### Benefits
- Catches transformation errors before deployment
- Validates environment-specific settings applied correctly
- Reduces deployment failures due to configuration issues

---

## 6. Parallel File Copy with Robocopy

### Current Issue
PowerShell `Copy-Item` can be slow for large directory structures (3000+ files).

### Recommendation
Replace manual copy with robocopy for better performance:

```yaml
- task: PowerShell@2
  displayName: Publish ALOD Web App (Robocopy)
  timeoutInMinutes: 5
  inputs:
    targetType: inline
    script: |
      $source = "$(Build.SourcesDirectory)\ALOD"
      $dest = "$(Build.ArtifactStagingDirectory)\ALOD"
      
      # Copy application files (multi-threaded)
      $result = robocopy "$source" "$dest" /E /XD obj bin "My Project" /XF *.vbproj *.user packages.config /MT:8 /NFL /NDL /NJH /NJS
      
      # Copy bin folder
      $binResult = robocopy "$source\bin" "$dest\bin" /E /MT:8 /NFL /NDL /NJH /NJS
      
      # Robocopy exit codes: 0-7 are success, 8+ are errors
      if ($result -ge 8 -or $binResult -ge 8) {
        throw "Robocopy failed with exit codes: main=$result, bin=$binResult"
      }
      
      Write-Host "Published ALOD web app to $dest"
      Get-ChildItem $dest -Recurse | Measure-Object | Select-Object -ExpandProperty Count | ForEach-Object { Write-Host "Total files copied: $_" }
```

### Benefits
- Multi-threaded copying (`/MT:8`)
- Faster for large directory structures
- More robust error handling
- Standard Windows tool (no dependencies)

---

## 7. Add Deployment Stage

### Purpose
Make the `deployToProd` parameter functional by adding an actual deployment stage.

### Recommendation
Add new stage after BuildAndPublish:

```yaml
# ==============================================================
# DEPLOYMENT (conditional on deployToProd parameter)
# ==============================================================
- stage: Deploy
  displayName: Deploy to Production IIS
  condition: and(succeeded(), eq('${{ parameters.deployToProd }}', true))
  dependsOn: BuildAndPublish
  jobs:
  - deployment: DeployToIIS
    displayName: Deploy ALOD to Production
    environment: 'Production'
    strategy:
      runOnce:
        deploy:
          steps:
          - download: current
            artifact: '$(artifactContainer)'
            
          - task: IISWebAppDeploymentOnMachineGroup@0
            displayName: Deploy to IIS
            inputs:
              WebSiteName: 'ALOD'
              Package: '$(Pipeline.Workspace)/$(artifactContainer)/$(artifactName).zip'
              RemoveAdditionalFilesFlag: true
              TakeAppOfflineFlag: true
```

### Requirements
- Configure Azure DevOps environment named "Production"
- Set up deployment group with IIS servers
- Configure appropriate approvals/gates on Production environment

### Benefits
- End-to-end CI/CD pipeline
- Environment-based deployment controls
- Audit trail for production deployments
- Rollback capabilities

---

## Priority Recommendations

### High Priority
1. **Security (#1)** - Remove hardcoded credentials immediately
2. **Build Validation (#2)** - Prevent bad artifacts from being published
3. **Config Validation (#5)** - Catch transformation errors early

### Medium Priority
4. **Artifact Size Reporting (#3)** - Track trends over time
5. **Robocopy Implementation (#6)** - Improve build performance

### Low Priority
6. **Documentation Fix (#4)** - Cosmetic consistency
7. **Deployment Stage (#7)** - Future enhancement when ready for automated deployments

---

## Implementation Notes

### Testing Strategy
1. Implement improvements in a feature branch
2. Test with non-production configurations first (Debug/Testing)
3. Validate artifact integrity after each change
4. Monitor build time impact

### Rollback Plan
All changes are additive or can be easily reverted by:
- Commenting out new tasks
- Reverting to previous pipeline version in Git

### Monitoring
After implementation, monitor:
- Build success rate
- Build duration (should decrease with #6)
- Artifact size trends (#3)
- Configuration validation failures (#5)

---

## Related Documentation

- [Build Configuration Report](Build_Configuration_Report.md)
- [VS Code Migration Summary](VS_Code_Migration_Summary.md)
- [Connection String Usage Analysis](Connection_String_Usage_Analysis.md)
