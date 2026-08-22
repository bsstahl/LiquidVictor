# LiquidVictor Quick Reference

## Version Management

### Check Current Version
```powershell
dotnet msbuild src\LiquidVictor\LiquidVictor.csproj -getProperty:Version -nologo
```

### Update Version for All Projects
1. Edit `src/Directory.Build.props`
2. Change `<Version>1.9.1</Version>` to your new version
3. Save and build

## Package Generation

### Generate All Packages at Once
```powershell
# Creates all .nupkg files in the packages directory
dotnet pack src\LiquidVictor.sln --configuration Release --output .\packages
```

### Generate Specific Packages
```powershell
# Domain layer package
dotnet pack src\LiquidVictor\LiquidVictor.csproj --configuration Release --output .\packages

# YamlFile data store package
dotnet pack src\LiquidVictor.Data.YamlFile\LiquidVictor.Data.YamlFile.csproj --configuration Release --output .\packages
```

### List Generated Packages
```powershell
Get-ChildItem .\packages\*.nupkg | Select-Object Name, Length
```

## Building the Solution

### Standard Build
```powershell
dotnet build src\LiquidVictor.sln --configuration Release
```

### Build with Package Generation
```powershell
# The packable projects will automatically generate packages
dotnet build src\LiquidVictor.sln --configuration Release
```

### Clean Build
```powershell
dotnet clean src\LiquidVictor.sln
dotnet build src\LiquidVictor.sln --configuration Release
```

## Publishing Packages

### Publish Single Package
```powershell
dotnet nuget push .\packages\LiquidVictor.1.9.1.nupkg --source https://api.nuget.org/v3/index.json --api-key YOUR_API_KEY
```

### Publish All Packages
```powershell
dotnet nuget push .\packages\*.nupkg --source https://api.nuget.org/v3/index.json --api-key YOUR_API_KEY
```

### Publish to Local Feed
```powershell
# Add a local source (one-time setup)
dotnet nuget add source C:\LocalNuGet --name LocalFeed

# Push to local feed
dotnet nuget push .\packages\*.nupkg --source LocalFeed
```

## Common Tasks

### Full Release Process
```powershell
# 1. Update version
# Edit src/Directory.Build.props and change <Version>

# 2. Clean build
dotnet clean src\LiquidVictor.sln
dotnet build src\LiquidVictor.sln --configuration Release

# 3. Generate all packages
dotnet pack src\LiquidVictor.sln --configuration Release --output .\packages

# 4. Verify packages
Get-ChildItem .\packages\*.nupkg | Select-Object Name

# 5. Publish (when ready)
dotnet nuget push .\packages\*.nupkg --source https://api.nuget.org/v3/index.json --api-key YOUR_API_KEY
```

### Development Build
```powershell
# Quick debug build
dotnet build src\LiquidVictor.sln

# Run main executable
dotnet run --project src\LV\LV.csproj
```

## Troubleshooting

### Build Errors Due to Code Analysis
If you encounter build errors from code analysis warnings:

```powershell
# Build with warnings as errors disabled (temporary)
dotnet build src\LiquidVictor.sln -p:TreatWarningsAsErrors=false
```

### Clear NuGet Cache
```powershell
dotnet nuget locals all --clear
```

### Restore Dependencies
```powershell
dotnet restore src\LiquidVictor.sln
```

## Project Structure

### Packable Projects (GeneratePackageOnBuild=true)
- `LiquidVictor` - Domain layer
- `LiquidVictor.Data.YamlFile` - YAML data store

### All Projects Share Common Properties From
- `src\Directory.Build.props` - Version, analysis settings, metadata
- `tst\Directory.Build.props` - Test-specific settings

### Executable Projects
- `LV` - Main CLI application (.NET 10 only)
- `LVExport` - Export utility (.NET 10 only)

### Data Layer Projects
- `LiquidVictor.Data.YamlFile` - File-based YAML storage
- `LiquidVictor.Data.Postgres` - PostgreSQL storage
- `LiquidVictor.Data.Hardcoded` - In-memory test data

### Output Projects
- `LiquidVictor.Output.RevealJs.*` - RevealJS presentation output
- `LiquidVictor.Output.Powerpoint.*` - PowerPoint output

## See Also
- [README.md](../README.md) - Main project documentation
- [VERSION_MANAGEMENT.md](./VERSION_MANAGEMENT.md) - Detailed version management guide
- [latex-support.md](./latex-support.md) - LaTeX math rendering guide
