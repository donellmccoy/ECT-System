param(
    [switch]$ALODOnly,
    [switch]$SRXLiteOnly
)

Write-Host 'Clearing browser cache...' -ForegroundColor Yellow

# Clear Windows INetCache
$inetCachePath = Join-Path $env:LOCALAPPDATA "Microsoft\Windows\INetCache"
if (Test-Path $inetCachePath) {
    Remove-Item -Path "$inetCachePath\*" -Recurse -Force -ErrorAction SilentlyContinue
}

# Clear Edge cache
$edgeCachePath = Join-Path $env:LOCALAPPDATA "Microsoft\Edge\User Data\Default\Cache"
if (Test-Path $edgeCachePath) {
    Remove-Item -Path "$edgeCachePath\*" -Recurse -Force -ErrorAction SilentlyContinue
}

Write-Host 'Cache cleared successfully' -ForegroundColor Green

# Launch IIS Express
$scriptPath = Join-Path $PSScriptRoot "launch-iisexpress.ps1"

if ($ALODOnly) {
    & $scriptPath -ALODOnly
}
elseif ($SRXLiteOnly) {
    & $scriptPath -SRXLiteOnly
}
else {
    & $scriptPath
}
