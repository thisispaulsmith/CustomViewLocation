// Install addins

// Install tools
#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=OctopusTools"
//#tool "nuget:?package=gitlink"

using Path = System.IO.Path;
using System.Text.RegularExpressions;

//////////////////////////////////////////////////////////////////////
// PARAMETERS
//////////////////////////////////////////////////////////////////////

var target = "Default";
var outputDirectory = "./output/";
var solutionPath = "./NetSmith.AspNetCore.CustomViewLocation.sln";
var testFolder = "./test";
var configuration = "release";

GitVersion gitVersionInfo;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
	
});

//////////////////////////////////////////////////////////////////////
// PRIVATE TASKS
//////////////////////////////////////////////////////////////////////

Task("__Clean")
	.Does(() =>
	{
		if (DirectoryExists(outputDirectory))
	    {
			DeleteDirectory(outputDirectory, recursive:true);
	    }
	    CreateDirectory(outputDirectory);
	});

Task("__Restore")
    .Does(() => 
	{
	    DotNetCoreRestore();
	});

Task("__Version")
    .Does(() => 
	{
	    // Update the build server
		GitVersion(new GitVersionSettings{
			OutputType = GitVersionOutput.BuildServer
		});
	
		gitVersionInfo = GitVersion(new GitVersionSettings {
			OutputType = GitVersionOutput.Json
		});
	});

Task("__Build")
    .Does(() => 
	{
        DotNetCoreBuild(solutionPath, new DotNetCoreBuildSettings
		{
			Configuration = configuration,
			ArgumentCustomization = args => args.Append("/p:SemVer=" + gitVersionInfo.NuGetVersion)
		});
    });

Task("__Test")
    .Does(() => 
	{
		var files = GetFiles(Path.Combine(testFolder, "*/*.csproj"));

		foreach(var file in files)
		{
			DotNetCoreTest(file.FullPath, new DotNetCoreTestSettings
			{
				Configuration = configuration,
			});
		}
    });

Task("__GitLink")
    .Does(() => 
	{
		//GitLink("./");
	});

Task("__Pack")
	.IsDependentOn("__GitLink")
    .Does(() => 
	{
		DotNetCorePack("./src/NetSmith.AspNetCore.CustomViewLocation/NetSmith.AspNetCore.CustomViewLocation.csproj", new DotNetCorePackSettings
        {
			Configuration = configuration,
            OutputDirectory = outputDirectory,
            NoBuild = true,
			IncludeSymbols = true,
			ArgumentCustomization = args => args.Append("/p:SemVer=" + gitVersionInfo.NuGetVersion)
        });
    });

Task("__Publish")
	.WithCriteria(() => AppVeyor.IsRunningOnAppVeyor)
    .Does(() => 
	{
		// Resolve the API key.
		var apiKey = EnvironmentVariable("NUGET_API_KEY");
	    if(string.IsNullOrEmpty(apiKey)) {
	        throw new InvalidOperationException("Could not resolve NuGet API key.");
	    }
	
	    // Resolve the API url.
	    var apiUrl = EnvironmentVariable("NUGET_API_URL");
	    if(string.IsNullOrEmpty(apiUrl)) {
	        throw new InvalidOperationException("Could not resolve NuGet API url.");
	    }

		foreach (var file in GetFiles(outputDirectory + "**/*"))
		{
			AppVeyor.UploadArtifact(file.FullPath);

			// Push the package.
			NuGetPush(package.PackagePath, new NuGetPushSettings {
				ApiKey = apiKey,
				Source = apiUrl
			});
		}
	});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Default")
	.IsDependentOn("__Clean")
	.IsDependentOn("__Version")
	.IsDependentOn("__Restore")
	.IsDependentOn("__Build")
	.IsDependentOn("__Test")
	.IsDependentOn("__Pack")
	.IsDependentOn("__Publish");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
