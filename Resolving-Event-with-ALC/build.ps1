param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Debug'
)

$target = Join-Path $PSScriptRoot 'bin'
if (Test-Path $target) {
    Remove-Item $target -Recurse -Force
}
$null = New-Item -ItemType Directory $target

try {
    $higherDeps = Join-Path $PSScriptRoot 'src' 'HigherDependencyConflict'
    Push-Location $higherDeps
    dotnet publish -c $Configuration 

    if ($LASTEXITCODE -eq 0) {
        $source = Join-Path $higherDeps 'bin' 'HigherConflict'
        Copy-Item $source -Recurse $target
    } else {
        return
    }
} finally {
    Pop-Location
}

try {
    $lowerDeps = Join-Path $PSScriptRoot 'src' 'LowerDependencyConflict'
    Push-Location -Path $lowerDeps
    dotnet publish -c $Configuration

    if ($LASTEXITCODE -eq 0) {
        $source = Join-Path $lowerDeps 'bin' 'LowerConflict'
        Copy-Item $source -Recurse $target
    } else {
        return
    }
} finally {
    Pop-Location
}

$sampleModule = Join-Path $PSScriptRoot 'src' 'SampleModule'
& "$sampleModule/build.ps1"
if ($?) {
    $source = Join-Path $sampleModule 'bin' 'SampleModule'
    Copy-Item $source -Recurse $target
}

Write-Host "`nAll modules are published to '$target'" -ForegroundColor Green
