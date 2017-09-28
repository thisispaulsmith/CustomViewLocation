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
			ArgumentCustomization = args => args.Append("/p:SemVer=" + gitVersionInfo.NuGetVersion)
        });
    });

Task("__Publish")
	.WithCriteria(() => AppVeyor.IsRunningOnAppVeyor)
    .Does(() => 
	{
		foreach (var file in GetFiles(outputDirectory + "**/*"))
		{
			AppVeyor.UploadArtifact(file.FullPath);
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
