# PowerShell script to remove special characters from .cs files
$files = Get-ChildItem -Path . -Recurse -Filter *.cs

foreach ($file in $files) {
    Write-Host "Cleaning $($file.FullName)"
    $content = Get-Content $file.FullName -Raw
    
    # Remove box-drawing characters and other special chars
    $content = $content -replace '[│└├┌─┐┘┴┬┤]', ''
    
    # Save the file
    $content | Set-Content $file.FullName -NoNewline
}

Write-Host "Cleanup complete!"
