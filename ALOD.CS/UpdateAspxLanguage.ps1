# Script to update all ASPX page directives from VB to C#
# Updates Language="VB" to Language="C#"
# Updates CodeBehind/Codebehind references from .vb to .cs

Write-Host "Updating ASPX page directives from VB to C#..." -ForegroundColor Cyan
Write-Host ""

$rootPath = "d:\source\repos\donellmccoy\ECT-System\ECT-System\ALOD.CS"
$aspxFiles = Get-ChildItem -Path $rootPath -Recurse -Filter "*.aspx"
$masterFiles = Get-ChildItem -Path $rootPath -Recurse -Filter "*.master"
$allFiles = $aspxFiles + $masterFiles

$updatedCount = 0
$totalCount = $allFiles.Count

foreach ($file in $allFiles) {
    $content = Get-Content $file.FullName -Raw -Encoding UTF8
    $originalContent = $content
    
    # Replace Language="VB" with Language="C#"
    $content = $content -replace 'Language="VB"', 'Language="C#"'
    
    # Replace Codebehind=".vb" with CodeBehind=".cs" (note capital B)
    $content = $content -replace 'Codebehind="([^"]+)\.vb"', 'CodeBehind="$1.cs"'
    
    # Replace CodeBehind=".vb" with CodeBehind=".cs"
    $content = $content -replace 'CodeBehind="([^"]+)\.vb"', 'CodeBehind="$1.cs"'
    
    # Replace CodeFile=".vb" with CodeFile=".cs"
    $content = $content -replace 'CodeFile="([^"]+)\.vb"', 'CodeFile="$1.cs"'
    
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline -Encoding UTF8
        $updatedCount++
        $relativePath = $file.FullName.Replace($rootPath, "").TrimStart("\")
        Write-Host "  ✓ $relativePath" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "═══════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "  Update Complete!" -ForegroundColor Green
Write-Host "═══════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""
Write-Host "Total files processed: $totalCount" -ForegroundColor White
Write-Host "Files updated: $updatedCount" -ForegroundColor Green
Write-Host "Files unchanged: $($totalCount - $updatedCount)" -ForegroundColor Gray
Write-Host ""

# Verify no VB references remain
Write-Host "Verification:" -ForegroundColor Yellow
$vbReferences = Get-ChildItem -Path $rootPath -Recurse -Include "*.aspx", "*.master" | Select-String 'Language="VB"' -SimpleMatch
$vbCount = ($vbReferences | Measure-Object).Count

if ($vbCount -eq 0) {
    Write-Host "  ✓ No VB language references found!" -ForegroundColor Green
}
else {
    Write-Host "  ⚠ Warning: $vbCount files still reference VB" -ForegroundColor Red
    $vbReferences | ForEach-Object { Write-Host "    - $($_.Path)" -ForegroundColor Yellow }
}
Write-Host ""
