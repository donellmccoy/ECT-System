# ALOD to ALOD.CS Batch Conversion Utility
# This script helps convert VB.NET code-behind files to C# skeleton files

param(
    [string]$ModulePath = "lod",
    [switch]$GenerateSkeletons = $false
)

$alodPath = "d:\source\repos\donellmccoy\ECT-System\ECT-System\ALOD\Secure\$ModulePath"
$alodCSPath = "d:\source\repos\donellmccoy\ECT-System\ECT-System\ALOD.CS\Secure\$ModulePath"

Write-Host "Processing module: $ModulePath"
Write-Host "Source: $alodPath"
Write-Host "Target: $alodCSPath"
Write-Host ""

# Get all VB files (excluding designers)
$vbFiles = Get-ChildItem -Path $alodPath -Filter "*.aspx.vb" -ErrorAction SilentlyContinue | 
    Where-Object { $_.Name -notlike "*designer*" }

Write-Host "Found $($vbFiles.Count) VB code-behind files to convert"
Write-Host ""

foreach ($vbFile in $vbFiles) {
    $csFileName = $vbFile.Name.Replace(".vb", ".cs")
    $csFilePath = Join-Path $alodCSPath $csFileName
    
    # Check if already exists
    if (Test-Path $csFilePath) {
        Write-Host "  [SKIP] $csFileName (already exists)"
        continue
    }
    
    Write-Host "  [TODO] $csFileName"
    
    if ($GenerateSkeletons) {
        # Read VB file to extract class name
        $vbContent = Get-Content $vbFile.FullName -Raw
        
        # Extract namespace (e.g., "Namespace Web.LOD")
        if ($vbContent -match 'Namespace\s+([^\r\n]+)') {
            $namespace = $matches[1].Trim()
        } else {
            $namespace = "ALOD.Web"
        }
        
        # Extract class name (e.g., "Partial Class Secure_lod_Audit")
        if ($vbContent -match 'Partial Class\s+([^\r\n]+)') {
            $className = $matches[1].Trim()
        } else {
            $className = $vbFile.BaseName.Replace(".aspx", "")
        }
        
        # Generate C# skeleton
        $csContent = @"
using System;
using System.Web.UI;

namespace $namespace
{
    public partial class $className : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO: Convert VB.NET logic from $($vbFile.Name)
        }
    }
}
"@
        
        # Write C# file
        Set-Content -Path $csFilePath -Value $csContent
        Write-Host "    Generated skeleton"
    }
}

Write-Host ""
Write-Host "Summary:"
Write-Host "  VB files found: $($vbFiles.Count)"
$existingCS = (Get-ChildItem -Path $alodCSPath -Filter "*.cs" -ErrorAction SilentlyContinue | Measure-Object).Count
Write-Host "  C# files exist: $existingCS"
Write-Host "  Remaining: $($vbFiles.Count - $existingCS)"
