using System.Collections.Generic;
using System.IO;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Nuget API key")] 
    readonly string ApiKey;

    [Parameter("NuGet Source for Packages", Name = "nuget-source")]
    readonly string NugetSource = "https://api.nuget.org/v3/index.json";
    
    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            DotNetClean(s => s
                .SetProject(Solution)
                .SetConfiguration(Configuration));
            
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                // .SetAssemblyVersion(GitVersion.AssemblySemVer)
                // .SetFileVersion(GitVersion.AssemblySemFileVer)
                // .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });
    
    IEnumerable<Project> TestProjects => Solution.GetProjects("*.Test");
    AbsolutePath TestResultDirectory => ArtifactsDirectory / "test-results";

    Target Test => _ => _
        .DependsOn(Compile)
        .Produces(TestResultDirectory / "*.trx")
        .Produces(TestResultDirectory / "*.xml")
        .Executes(() =>
        {
            DotNetTest(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .SetNoRestore(InvokedTargets.Contains(Restore))
                .SetResultsDirectory(TestResultDirectory)
                .When(InvokedTargets.Contains(Coverage) || IsServerBuild, _ => _
                    .EnableCollectCoverage()
                    .SetCoverletOutputFormat(CoverletOutputFormat.opencover)
                    .When(IsServerBuild, _ => _.EnableUseSourceLink()))
                .CombineWith(TestProjects, (_, v) => _
                    .SetProjectFile(v)
                    .SetLoggers($"trx;LogFileName={v.Name}.trx")
                    .SetCoverletOutput(TestResultDirectory / $"{v.Name}.xml"))
            );
        });
    
    string CoverageReportDirectory => ArtifactsDirectory / "coverage-report";
    string CoverageReportArchive => ArtifactsDirectory / "coverage-report.zip";
    
    Target Coverage => _ => _
        .DependsOn(Test)
        .TriggeredBy(Test)
        .Consumes(Test)
        .Produces(CoverageReportArchive)
        .Executes(() =>
        {
            if (InvokedTargets.Contains(Coverage) || IsServerBuild)
            {
                ReportGenerator(_ => _
                    .SetReports(TestResultDirectory / "*.xml")
                    .SetReportTypes(ReportTypes.HtmlInline)
                    .SetTargetDirectory(CoverageReportDirectory)
                    .SetFramework("netcoreapp2.1"));
            }
        });
    
    AbsolutePath PackageDirectory => ArtifactsDirectory / "packages";
    Target Pack => _ => _
        .DependsOn(Compile)
        .Produces(PackageDirectory / "*.nupkg")
        .Produces(PackageDirectory / "*.snupkg")
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetProject(Solution)
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .SetNoRestore(InvokedTargets.Contains(Restore))
                .SetConfiguration(Configuration)
                .SetOutputDirectory(PackageDirectory)
                .SetVersion(GitVersion.NuGetVersionV2)
                .EnableIncludeSource()
                .EnableIncludeSymbols());
        });
    
    Target Publish => _ => _
        .DependsOn(Pack)
        .Consumes(Pack)
        .Requires(() => ApiKey)
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Executes(() =>
        {
            DotNetNuGetPush(s => s
                .SetApiKey(ApiKey)
                .EnableSkipDuplicate()
                .SetSource(NugetSource)
                .SetTargetPath(PackageDirectory / "*.nupkg"));
            
            DotNetNuGetPush(s => s
                .SetApiKey(ApiKey)
                .EnableSkipDuplicate()
                .SetSource(NugetSource)
                .SetTargetPath(PackageDirectory / "*.snupkg"));
        });

}
