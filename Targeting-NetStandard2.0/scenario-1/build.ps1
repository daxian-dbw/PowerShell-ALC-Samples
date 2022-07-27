param(
    [ValidateSet("Debug", "Release")]
    [string] $Configuration = "Debug"
)

dotnet publish -c $Configuration

if ($LASTEXITCODE -eq 0) {
    $target = Join-Path $PSScriptRoot 'bin' 'conflict' 'Dependencies'
    if (Test-Path $target) {
        Remove-Item $target -Recurse -Force
    }

    $null = New-Item -ItemType Directory $target
    Move-Item "$PSScriptRoot\bin\conflict\Newtonsoft.Json.dll" $target

    $moduleDir = Join-Path $PSScriptRoot 'bin' 'conflict'
    $moduleDir = Resolve-Path $moduleDir -Relative
    Write-Host "  Module deployed to: $moduleDir" -ForegroundColor Green
}
