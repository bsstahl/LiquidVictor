#tool "nuget:?package=xunit.runner.console"
#addin "Cake.Docker"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/LiquidVictor.Output.RevealJs.Layout.Service/bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore(".");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
      // MSBuild("./LiquidVictor.sln", settings => settings.SetConfiguration(configuration));
	  DotNetCoreBuild("./LiquidVictor.sln",
		new DotNetCoreBuildSettings()
		{
			Configuration = configuration,
			ArgumentCustomization = args => args.Append("--no-restore")
		});
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
	var projectFiles = GetFiles("./tst/**/*.csproj");
    foreach(var file in projectFiles)
    {
		Information($"Running tests in '{file.FullPath}'");
        DotNetCoreTest(file.FullPath);
    }
});

Task("Publish")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
    {
        // LiquidVictor CLI
        DotNetCorePublish("./src/LV", 
            new DotNetCorePublishSettings() {
                Configuration = configuration,
                ArgumentCustomization = args => args.Append("--no-restore")
            });

        // Base Layout Service
        DotNetCorePublish("./src/LiquidVictor.Output.RevealJs.Layout.Service", 
            new DotNetCorePublishSettings() {
                Configuration = configuration,
                ArgumentCustomization = args => args.Append("--no-restore")
            });
    });

Task("Containerize")
	.IsDependentOn("Publish")
	.Does(() =>
	{
		DockerBuild(
            new DockerImageBuildSettings() 
            {
                Tag = new string[] {"liquidvictorlayoutservicebase:latest"}
            },
            "./src/LiquidVictor.Output.RevealJs.Layout.Service"
        );
	});


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Run-Unit-Tests");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

Information($"Running target {target} in configuration {configuration}");
RunTarget(target);
