param(
    [switch]$IncludeSqlServerIntegration
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
    dotnet --info
    dotnet tool restore
    dotnet restore AcademyOps.sln
    dotnet build AcademyOps.sln --configuration Release --no-restore

    if ($IncludeSqlServerIntegration) {
        dotnet test AcademyOps.sln --configuration Release --no-build
    }
    else {
        dotnet test AcademyOps.sln `
            --configuration Release `
            --no-build `
            --filter "Category!=SqlServerIntegration"
    }

    dotnet tool run dotnet-ef migrations list `
        --project src/AcademyOps/AcademyOps.Infrastructure.EntityFrameworkCore.SqlServer `
        --startup-project src/AcademyOps/AcademyOps.Api `
        --no-connect
}
finally {
    Pop-Location
}
