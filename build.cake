#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0

// Cake Addins
#addin nuget:?package=Cake.FileHelpers&version=2.0.0

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var VERSION = "5.0.0";
var PATCH = "";

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var slnPath = "./accountkit-droid.sln";
var artifacts = new [] {
    new Artifact {
        Name = "",
        Version = VERSION,
        AssemblyInfoPath = "./Naxam.AccountKit.Droid/Properties/AssemblyInfo.cs",
        NuspecPath = "./accountkit.nuspec",
        DownloadUrl = "https://bintray.com/bintray/jcenter/download_file?file_path=com%2Ffacebook%2Fandroid%2Faccount-kit-sdk%2F{0}%2Faccount-kit-sdk-{0}.aar",
        JarPath = "./Naxam.AccountKit.Droid/Jars/account-kit-sdk.aar"
    },
};

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Downloads")
    .Does(() =>
{
    foreach(var artifact in artifacts) {
        var downloadUrl = string.Format(artifact.DownloadUrl, artifact.Version);
        var jarPath = string.Format(artifact.JarPath, artifact.Version);

        DownloadFile(downloadUrl, jarPath);
    }
});

Task("Clean")
    .Does(() =>
{
    CleanDirectory("./packages");

    var nugetPackages = GetFiles("./*.nupkg");

    foreach (var package in nugetPackages)
    {
        DeleteFile(package);
    }
});

Task("Restore-NuGet-Packages")
    .Does(() =>
{
    NuGetRestore(slnPath);
});

Task("Build")
    .Does(() =>
{
    MSBuild(slnPath, settings => settings.SetConfiguration(configuration));
});

Task("UpdateVersion")
    .Does(() => 
{
    foreach(var artifact in artifacts) {
        ReplaceRegexInFiles(artifact.AssemblyInfoPath, "\\[assembly\\: AssemblyVersion([^\\]]+)\\]", string.Format("[assembly: AssemblyVersion(\"{0}\")]", artifact.Version + PATCH));
    }
});

Task("Pack")
    .Does(() =>
{
    foreach(var artifact in artifacts) {
        NuGetPack(artifact.NuspecPath, new NuGetPackSettings {
            Version = artifact.Version + PATCH,
            Title = $"AccountKit for Android",
            Description = $"Xamarin.Android binding library - AccountKit",
            Summary = $"Xamarin.Android binding library - AccountKit",
            // Dependencies = artifact.Dependencies,
            ReleaseNotes = new [] {
                $"AccountKit for Android - v{artifact.Version}"
            }
        });
    }
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Downloads")
    .IsDependentOn("UpdateVersion")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Build")
    .IsDependentOn("Pack");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);

class Artifact {
    public string AssemblyInfoPath { get; set; }

    public string SolutionPath { get; set; }

    public string DownloadUrl  { get; set; }

    public string JarPath { get; set; }

    public string NuspecPath { get; set; }

    public string Version { get; set; }

    public string ReleaseNote { get; set; }

    public string Name { get; set; }

    public NuSpecDependency[] Dependencies { get; set; } = new NuSpecDependency[0];
}