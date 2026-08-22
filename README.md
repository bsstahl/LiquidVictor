# LiquidVictor

**A media tracking and aggregation system that can be used to build presentations.**

## Code of Conduct

All contributors and users are expected to follow the [Strict Accountability Policy](CODE_OF_CONDUCT.md).

The goal of *LiquidVictor* is to make building presentations easier.  Many presentations need to 
evolve over time but often use assets from previous versions or even other presentations. 
If presentation assets can be organized and tracked so that the basic structure of presentations 
can be put together by selecting and ordering these existing assets, it would make creating 
and maintaining these presentations much easer.

* Assets that could be tracked include:
  * Slide content
  * Images
  * Charts and graphs
  * Links and their thumbnails
  * Code samples

* Some key features of the system may be:
  * Generate presentations in multiple formats and with different themes and aspect ratios
  * Allow the regeneration of all presentations containing a slide or image when that asset changes
  * Support the labeling of states so that a presentation can be regenerated as it was at a particular point in time
  * Render LaTeX mathematical expressions in slide content using MathJax

## Features

### LaTeX Math Support

Slide content written in Markdown can include LaTeX mathematical expressions using standard dollar-sign delimiters:

* **Inline math** — wrap an expression in single dollar signs: `$E = mc^2$`
* **Display math** — place double dollar signs on their own lines above and below the expression:
  ```
  $$
  \frac{-b \pm \sqrt{b^2 - 4ac}}{2a}
  $$
  ```

LaTeX is rendered in the browser by [MathJax](https://www.mathjax.org/) through the RevealJS math support. In the default configuration, MathJax is loaded from a CDN rather than bundled locally, so viewing rendered math requires network access unless you reconfigure it. See [docs/latex-support.md](docs/latex-support.md) for a comprehensive guide including more examples and links to LaTeX documentation.

## Building the Project

Prototypes and contribution guidelines are forthcoming.

To build the project, take the following steps:

1) Download the code repository and open the LiquidVictor solution file in Visual Studio or equivalent IDE.
1) From a console window at the root of the code repository, execute the git command `git update-index --skip-worktree \src\lv\Properties\launchSettings.json` to tell git not to upload any changes to that file since they are specific to your local installation.
1) Modify the `\src\lv\Properties\launchSettings.json` file to point to the presentation you wish to build (sample presentation repositories are forthcoming).
1) Execute the LV.exe CLI by pressing F5 in Visual Studio or invoking the proper command in your IDE.

## Version Management

**LiquidVictor uses centralized version management** to ensure all projects share the same version number and build settings.

### Current Version
The current version is managed in `src/Directory.Build.props` and is automatically inherited by all projects under the `src/` directory.

### Updating the Version
To update the version for all projects:

1. Edit `src/Directory.Build.props`
2. Update the `<Version>` property:
   ```xml
   <Version>1.10.0</Version>
   ```
3. Optionally update `<AssemblyVersion>` and `<FileVersion>` to match
4. Build the solution to apply the changes

### Verifying the Version
To check the version being used by a specific project:

```powershell
dotnet msbuild path\to\project.csproj -getProperty:Version -nologo
```

For more details, see [docs/VERSION_MANAGEMENT.md](docs/VERSION_MANAGEMENT.md).

For a quick reference of common commands, see [docs/QUICK_REFERENCE.md](docs/QUICK_REFERENCE.md).

## Package Generation

The following projects are configured to generate NuGet packages:

- **LiquidVictor** - Domain layer for media tracking and aggregation
- **LiquidVictor.Data.YamlFile** - YAML file-based data store implementation

### Generating Packages

#### Option 1: Generate All Packages at Once
To generate NuGet packages for all packable projects in a single command:

```powershell
dotnet pack src\LiquidVictor.sln --configuration Release --output .\packages
```

This will create `.nupkg` files in the `packages` directory at the solution root.

#### Option 2: Generate Individual Project Packages
To generate a package for a specific project:

```powershell
# LiquidVictor domain package
dotnet pack src\LiquidVictor\LiquidVictor.csproj --configuration Release --output .\packages

# YamlFile data store package
dotnet pack src\LiquidVictor.Data.YamlFile\LiquidVictor.Data.YamlFile.csproj --configuration Release --output .\packages
```

#### Option 3: Automatic Generation on Build
Both packable projects have `GeneratePackageOnBuild` enabled. Building these projects in **Release** configuration will automatically generate NuGet packages in their respective `bin\Release\` folders.

### Package Output Location
- **Explicit pack command with --output**: Packages are created in the specified directory
- **Automatic pack on build**: Packages are created in `bin\Release\{framework}\` for each target framework

### Publishing Packages
To publish packages to NuGet.org or a private feed:

```powershell
dotnet nuget push .\packages\LiquidVictor.1.9.1.nupkg --source https://api.nuget.org/v3/index.json --api-key YOUR_API_KEY
```

Replace `YOUR_API_KEY` with your actual NuGet API key.

