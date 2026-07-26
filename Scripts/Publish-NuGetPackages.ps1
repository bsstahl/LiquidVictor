[CmdletBinding()]
param(
    [string]$ApiKey,

    [string]$Source = "https://api.nuget.org/v3/index.json",

    [string]$PackageOutputPath = "C:\LocalRepo",

    [switch]$DryRun
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Invoke-DotNet {
    param(
        [string[]]$CommandArgs
    )

    & dotnet @CommandArgs
    if ($LASTEXITCODE -ne 0) {
        $safeArgs = @()
        for ($i = 0; $i -lt $CommandArgs.Count; $i++) {
            $safeArgs += $CommandArgs[$i]
            if ($CommandArgs[$i] -eq "--api-key" -and ($i + 1) -lt $CommandArgs.Count) {
                $safeArgs += "***"
                $i++
            }
        }

        throw "dotnet command failed with exit code ${LASTEXITCODE}: dotnet $($safeArgs -join ' ')"
    }
}

function Resolve-ApiKey {
    param(
        [string]$ProvidedApiKey
    )

    if (-not [string]::IsNullOrWhiteSpace($ProvidedApiKey)) {
        return $ProvidedApiKey
    }

    if (-not [string]::IsNullOrWhiteSpace($env:NUGET_API_KEY)) {
        Write-Host "Using API key from environment variable NUGET_API_KEY." -ForegroundColor Cyan
        return $env:NUGET_API_KEY
    }

    if (-not [string]::IsNullOrWhiteSpace($env:NUGET_ORG_API_KEY)) {
        Write-Host "Using API key from environment variable NUGET_ORG_API_KEY." -ForegroundColor Cyan
        return $env:NUGET_ORG_API_KEY
    }

    Write-Host "No NuGet API key was provided and no key environment variable was found." -ForegroundColor Yellow
    $entered = Read-Host "Enter NuGet API key (used for this run only)"
    if ([string]::IsNullOrWhiteSpace($entered)) {
        throw "A NuGet API key is required to publish packages."
    }

    return $entered
}

$ApiKey = Resolve-ApiKey -ProvidedApiKey $ApiKey

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$outputPath = [System.IO.Path]::GetFullPath($PackageOutputPath)

if (-not (Test-Path $outputPath)) {
    New-Item -ItemType Directory -Path $outputPath | Out-Null
}

$projects = @(
    @{
        PackageId = "LiquidVictor"
        ProjectPath = (Join-Path $repoRoot "src\LiquidVictor\LiquidVictor.csproj")
    },
    @{
        PackageId = "LiquidVictor.Data.YamlFile"
        ProjectPath = (Join-Path $repoRoot "src\LiquidVictor.Data.YamlFile\LiquidVictor.Data.YamlFile.csproj")
    }
)

$generatedPackages = @()

foreach ($project in $projects) {
    Write-Host "Packing $($project.PackageId) from $($project.ProjectPath)" -ForegroundColor Cyan

    $packArgs = @(
        "pack"
        $project.ProjectPath
        "-c"
        "Release"
        "-p:PackageOutputPath=$outputPath"
    )

    if ($DryRun) {
        Write-Host "[DryRun] dotnet $($packArgs -join ' ')" -ForegroundColor Yellow
    }
    else {
        Invoke-DotNet -CommandArgs $packArgs
    }

    $packageNamePattern = "^{0}\.[0-9].*\.nupkg$" -f [Regex]::Escape($project.PackageId)
    $package = Get-ChildItem -Path $outputPath -Filter "*.nupkg" |
        Where-Object {
            $_.Name -match $packageNamePattern -and
            -not $_.Name.EndsWith(".symbols.nupkg", [System.StringComparison]::OrdinalIgnoreCase)
        } |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1

    if ($null -eq $package) {
        throw "Could not find a package file for $($project.PackageId) in $outputPath."
    }

    $generatedPackages += $package.FullName
}

Write-Host "Resolved package files:" -ForegroundColor Cyan
$generatedPackages | ForEach-Object { Write-Host " - $_" }

foreach ($packagePath in $generatedPackages) {
    $pushArgs = @(
        "nuget"
        "push"
        $packagePath
        "--api-key"
        $ApiKey
        "--source"
        $Source
        "--skip-duplicate"
    )

    if ($DryRun) {
        $maskedPushArgs = @(
            "nuget"
            "push"
            $packagePath
            "--api-key"
            "***"
            "--source"
            $Source
            "--skip-duplicate"
        )
        Write-Host "[DryRun] dotnet $($maskedPushArgs -join ' ')" -ForegroundColor Yellow
    }
    else {
        Write-Host "Publishing $packagePath" -ForegroundColor Cyan
        Invoke-DotNet -CommandArgs $pushArgs
    }
}

Write-Host "Done." -ForegroundColor Green
