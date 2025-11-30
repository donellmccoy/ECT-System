param(
    [Parameter(Mandatory=$true)]
    [string]$SourceConfig,
    
    [Parameter(Mandatory=$true)]
    [string]$TransformConfig,
    
    [Parameter(Mandatory=$true)]
    [string]$DestinationConfig
)

# Load Microsoft.Web.XmlTransform assembly
$dllPath = Get-ChildItem -Path "C:\Program Files\dotnet\sdk" -Recurse -Filter "Microsoft.Web.XmlTransform.dll" -ErrorAction SilentlyContinue | Select-Object -First 1 | Select-Object -ExpandProperty FullName

if (-not $dllPath) {
    Write-Host "ERROR: Microsoft.Web.XmlTransform.dll not found!" -ForegroundColor Red
    exit 1
}

Add-Type -LiteralPath $dllPath -ErrorAction Stop

Write-Host "Applying transform..." -ForegroundColor Cyan
Write-Host "  Source: $SourceConfig"
Write-Host "  Transform: $TransformConfig"
Write-Host "  Destination: $DestinationConfig"

$xmldoc = New-Object Microsoft.Web.XmlTransform.XmlTransformableDocument
$xmldoc.PreserveWhitespace = $true
$xmldoc.Load($SourceConfig)

$transf = New-Object Microsoft.Web.XmlTransform.XmlTransformation($TransformConfig)

if ($transf.Apply($xmldoc))
{
    $xmldoc.Save($DestinationConfig)
    Write-Host "Transform applied successfully!" -ForegroundColor Green
}
else
{
    Write-Host "Transform failed!" -ForegroundColor Red
    exit 1
}
