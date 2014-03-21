using System;
using System.Collections.Generic;
using NUnit.Framework;
using Octokit;
using SyncOMatic;

[TestFixture]
[Explicit]
public class SyncRepos
{

    //[Test]
    //public void Sync()
    //{
    //    using (var syncer = new Syncer(GetCredentials(),null, ConsoleLogger))
    //    {
    //        foreach (var map in Mapper())
    //        {
    //            var diff = syncer.Diff(map);
    //            foreach (var output in  syncer.Sync(diff, SyncOutput.CreatePullRequest))
    //            {
    //                if (!string.IsNullOrEmpty(output))
    //                {
    //                    Console.Out.WriteLine("Done " + output);
    //                }
    //            }
    //        }
    //    }
    //}

    static IEnumerable<Mapper> Mapper()
    {
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("AsyncErrorHandler", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("AsyncErrorHandler", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("Anotar", "install.ps1"))
                .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("Anotar", "uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("AssertMessage", "NuGet/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("AssertMessage", "NuGet/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("BasicFodyAddin", "NuGet/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("BasicFodyAddin", "NuGet/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("Caseless", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("Caseless", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("EmptyConstructor", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("EmptyConstructor", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("ExtraConstraints", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("ExtraConstraints", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("Equals", "NuGet/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("Equals", "NuGet/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("Fielder", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("Fielder", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("Freezable", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("Freezable", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("Immutable", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("Immutable", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("InfoOf", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("InfoOf", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("Ionad", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("Ionad", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("Janitor", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("Janitor", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("JetBrainsAnnotations", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("JetBrainsAnnotations", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("MethodDecorator", "install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("MethodDecorator", "uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("MethodTimer", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("MethodTimer", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("ModuleInit", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("ModuleInit", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("Obsolete", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("Obsolete", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("PropertyChanged", "Nuget/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("PropertyChanged", "Nuget/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("PropertyChanging", "Nuget/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("PropertyChanging", "Nuget/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("Publicize", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("Publicize", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("Resourcer", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("Resourcer", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("Scalpel", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("Scalpel", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("Stamp", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("Stamp", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("ToString", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("ToString", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("Validar", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("Validar", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("Virtuosity", "Fody/NugetAssets/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("Virtuosity", "Fody/NugetAssets/uninstall.ps1"));
        yield return new Mapper()
            .Add(SourceBlob("WeaverNugetTemplates/install.ps1"), TargetBlob("Visualize", "Nuget/install.ps1"))
            .Add(SourceBlob("WeaverNugetTemplates/uninstall.ps1"), TargetBlob("Visualize", "Nuget/uninstall.ps1"));
    }

    static Parts TargetBlob(string repo, string nugetInstallPs1)
    {
        return new Parts("Fody/" + repo, TreeEntryTargetType.Blob, "master", nugetInstallPs1);
    }

    static Parts SourceBlob(string path)
    {
        return new Parts( "Fody/Fody", TreeEntryTargetType.Blob, "master", path);
    }

    internal static void ConsoleLogger(LogEntry obj)
    {
        Console.WriteLine("{0}\t{1}", obj.At.ToString("o"), obj.What);
    }

    static Credentials GetCredentials()
    {
        var githubUsername = Environment.GetEnvironmentVariable("OCTOKIT_GITHUBUSERNAME");

        var githubToken = Environment.GetEnvironmentVariable("OCTOKIT_OAUTHTOKEN");

        if (githubToken != null)
            return new Credentials(githubToken);

        var githubPassword = Environment.GetEnvironmentVariable("OCTOKIT_GITHUBPASSWORD");

        if (githubUsername == null || githubPassword == null)
            return Credentials.Anonymous;

        return new Credentials(githubUsername, githubPassword);
    }
}
