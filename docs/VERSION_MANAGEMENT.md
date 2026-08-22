# LiquidVictor Centralized Version Management

## Overview
This document describes the centralized version management implementation for the LiquidVictor solution using `Directory.Build.props`.

## Implementation Date
Created: 2026-06-14
Initial Version: 1.9.1

## Structure

### Directory.Build.props Location
`src/Directory.Build.props`

This file is automatically imported by all `.csproj` files in the `src/` directory and its subdirectories.

## Centralized Properties

### Version Management
- **Version**: 1.9.1
- **AssemblyVersion**: 1.9.1.0
- **FileVersion**: 1.9.1.0

### Common Build Settings
- LangVersion: latest
- Nullable: enable
- ImplicitUsings: enable
- TreatWarningsAsErrors: True
- AnalysisLevel: latest-all
- AnalysisMode: all
- EnableNETAnalyzers: True

### Package Metadata
- Authors: Barry Stahl
- Company: Barry Stahl
- Copyright: GNU General Public License v3.0
- PackageProjectUrl: https://github.com/bsstahl/liquidvictor
- RepositoryUrl: https://github.com/bsstahl/liquidvictor
- RepositoryType: git
- PackageLicenseExpression: GPL-3.0-only

## Benefits

1. **Single Source of Truth**: All version numbers are managed in one location
2. **Consistency**: All projects use the same version and build settings
3. **Simplified Updates**: Change version in one file to update all projects
4. **Reduced Duplication**: Common properties don't need to be repeated in each project
5. **Easier Maintenance**: Cleaner, more focused project files

## Updating Versions

To update the version for all projects:

1. Edit `src/Directory.Build.props`
2. Update the `<Version>` property (e.g., `<Version>1.10.0</Version>`)
3. Optionally update `<AssemblyVersion>` and `<FileVersion>` to match
4. Build the solution

## Projects Affected

All projects under the `src/` directory now inherit these settings:

- LiquidVictor (domain layer)
- LiquidVictor.Business
- LiquidVictor.Data.YamlFile
- LiquidVictor.Data.Postgres
- LiquidVictor.Data.Hardcoded
- LiquidVictor.Output.* (all output projects)
- LiquidVictor.Strategy.*
- LV (executable)
- LVExport (executable)

## Project-Specific Overrides

Individual projects can still override centralized properties if needed by defining them in their own `.csproj` file. Project-level properties take precedence over Directory.Build.props.

### Example Override
```xml
<PropertyGroup>
  <Version>2.0.0-beta</Version>  <!-- Overrides central version -->
</PropertyGroup>
```

## Verification

To verify the version being used by a specific project:

```powershell
dotnet msbuild path\to\project.csproj -getProperty:Version -nologo
```

Expected output: `1.9.1`

## Migration Notes

### Changes Made
- Created `src/Directory.Build.props` with centralized properties
- Removed duplicate properties from individual `.csproj` files
- Projects now only contain project-specific settings (TargetFrameworks, OutputType, etc.)
- Previous versions: LiquidVictor (1.8.0), LiquidVictor.Data.YamlFile (1.9.0) → now all 1.9.1

### Preserved Properties
Each project still maintains:
- TargetFrameworks (or TargetFramework for executables)
- OutputType (for executables)
- Project-specific settings (UserSecretsId, GeneratePackageOnBuild, etc.)
- Package-specific metadata (Title, Description)
- Dependencies (PackageReference, ProjectReference)

## Build Issues Note

After implementing centralized version management, existing code analysis warnings are now being enforced across all projects due to the `TreatWarningsAsErrors=True` setting. These are pre-existing code quality issues unrelated to the version management implementation.

To address build failures, either:
1. Fix the code analysis warnings in the affected files
2. Temporarily disable warnings as errors for specific projects
3. Suppress specific analyzer rules in a `.editorconfig` or project file

## Package Generation

### Packable Projects

The following projects are configured with `GeneratePackageOnBuild=true`:
- **LiquidVictor** - Domain layer
- **LiquidVictor.Data.YamlFile** - YAML file data store

All other projects can still be packaged on-demand using `dotnet pack`.

### Generating All Packages

To generate NuGet packages for all projects at once:

```powershell
# From the solution root directory
dotnet pack src\LiquidVictor.sln --configuration Release --output .\packages
```

This creates all `.nupkg` files in the `packages` directory with the centralized version number (e.g., `LiquidVictor.1.9.1.nupkg`).

### Generating Individual Packages

```powershell
# Single project
dotnet pack src\LiquidVictor\LiquidVictor.csproj --configuration Release --output .\packages

# YamlFile data store
dotnet pack src\LiquidVictor.Data.YamlFile\LiquidVictor.Data.YamlFile.csproj --configuration Release --output .\packages
```

### Package Version

All generated packages automatically use the version defined in `src/Directory.Build.props`. When you update the central version, all packages will use the new version on the next build.

### Publishing to NuGet

```powershell
dotnet nuget push .\packages\*.nupkg --source https://api.nuget.org/v3/index.json --api-key YOUR_API_KEY
```
