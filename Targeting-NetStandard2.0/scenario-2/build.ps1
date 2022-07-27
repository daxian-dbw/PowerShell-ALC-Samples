param(
    [ValidateSet("Debug", "Release")]
    [string] $Configuration = "Debug"
)

try {
    Push-Location $PSScriptRoot/conflict
    dotnet publish -c $Configuration
} finally {
    Pop-Location
}

if ($LASTEXITCODE -eq 0) {
    try {
        Push-Location $PSScriptRoot/resolver
        dotnet publish -c $Configuration
    } finally {
        Pop-Location
    }

    $moduleDir = Join-Path $PSScriptRoot 'bin' 'conflict'
    $target = Join-Path $PSScriptRoot 'bin' 'conflict' 'Dependencies'
    if (Test-Path $target) {
        Remove-Item $target -Recurse -Force
    }

    $null = New-Item -ItemType Directory $target
    Move-Item "$PSScriptRoot\bin\conflict\Newtonsoft.Json.dll" $target
    Move-Item "$PSScriptRoot\bin\resolver\resolver.dll" $moduleDir
    Remove-Item "$PSScriptRoot\bin\resolver" -Recurse

    $moduleDir = Resolve-Path $moduleDir -Relative
    Write-Host "  Module deployed to: $moduleDir" -ForegroundColor Green
}
