$ErrorActionPreference = 'Stop'

# TUnit test projects are executables. They are run with 'dotnet run' for each target framework.
$projects = @(
    'Tests\Tests.csproj',
    'FodyHelpers.Tests\FodyHelpers.Tests.csproj',
    'Integration\Tests\Tests.csproj',
    'Integration\SampleTargetTests\SampleTargetTests.csproj',
    'Integration\SampleTargetWithConfigOverrideTests\SampleTargetWithConfigOverrideTests.csproj'
)

$failed = @()
foreach ($project in $projects) {
    $frameworks = (dotnet msbuild $project -getProperty:TargetFrameworks -p:Configuration=Release).Trim().Split(';')
    foreach ($framework in $frameworks) {
        Write-Host "Running $project ($framework)"
        dotnet run --project $project -c Release -f $framework --no-build
        if ($LASTEXITCODE -ne 0) {
            $failed += "$project ($framework)"
        }
    }
}

if ($failed.Count -gt 0) {
    Write-Host "Failed test runs:`n$($failed -join "`n")"
    exit 1
}
