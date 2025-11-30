# Launch both ALOD and SRXLite websites with IIS Express
param(
    [string]$ConfigPath = "$PSScriptRoot\..\.vs\AFLOD\config\applicationhost.config",
    [switch]$ALODOnly,
    [switch]$SRXLiteOnly
)

$IISExpressPath = "C:\Program Files\IIS Express\iisexpress.exe"

Write-Host "🚀 Starting IIS Express for ECT System websites..." -ForegroundColor Cyan
Write-Host "Configuration: $ConfigPath" -ForegroundColor Gray

# Stop any existing IIS Express processes
$existingProcesses = Get-Process iisexpress -ErrorAction SilentlyContinue
if ($existingProcesses) {
    Write-Host "⏹️  Stopping existing IIS Express processes..." -ForegroundColor Yellow
    $existingProcesses | Stop-Process -Force
    Start-Sleep 2
}

try {
    if ($ALODOnly) {
        Write-Host "🌐 Starting ALOD website only..." -ForegroundColor Green
        & $IISExpressPath /config:"$ConfigPath" /site:ALOD
    }
    elseif ($SRXLiteOnly) {
        Write-Host "🌐 Starting SRXLite website only..." -ForegroundColor Green  
        & $IISExpressPath /config:"$ConfigPath" /site:SRXLite
    }
    else {
        Write-Host "🌐 Starting both websites..." -ForegroundColor Green
        Write-Host "   • ALOD: http://localhost:44300" -ForegroundColor White
        Write-Host "   • SRXLite: http://localhost:44301/DocumentService.asmx" -ForegroundColor White
        
        # Start ALOD in background
        Start-Process -FilePath $IISExpressPath -ArgumentList "/config:`"$ConfigPath`"", "/site:ALOD" -WindowStyle Minimized
        Start-Sleep 2
        
        # Start SRXLite in foreground (this will keep the terminal open)
        & $IISExpressPath /config:"$ConfigPath" /site:SRXLite
    }
}
catch {
    Write-Host "❌ Error starting IIS Express: $_" -ForegroundColor Red
    exit 1
}