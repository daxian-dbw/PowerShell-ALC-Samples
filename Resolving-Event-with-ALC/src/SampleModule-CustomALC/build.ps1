param(
    [ValidateSet("Debug", "Release")]
    [string] $Configuration = "Debug"
)

dotnet publish "$PSScriptRoot\SampleModule-CustomALC.sln"

if ($LASTEXITCODE -eq 0) {
    $target = Join-Path $PSScriptRoot 'bin' 'SampleModule' 'Dependencies'
    if (Test-Path $target) {
        Remove-Item $target -Recurse -Force
    }

    $null = New-Item -ItemType Directory $target
    Move-Item "$PSScriptRoot\bin\SampleModule\SharedDependency.dll" $target
}
