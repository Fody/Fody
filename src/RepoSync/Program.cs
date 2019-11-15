using System;
using System.Threading.Tasks;
using GitHubSync;
using Octokit;

class Program
{
    static Task Main()
    {
        var githubToken = Environment.GetEnvironmentVariable("Octokit_OAuthToken");

        var credentials = new Credentials(githubToken);

        var sync = new RepoSync(
            log: Console.WriteLine,
            defaultCredentials: credentials);

        sync.AddSourceRepository(
            owner: "Fody",
            repository: "Home",
            branch: "master");

        //sync.AddSourceItem(TreeEntryTargetType.Blob, "RepoSync/appveyor.yml", "appveyor.yml");
        sync.AddSourceItem(TreeEntryTargetType.Blob, "src/RepoSync/Source/.editorconfig", ".editorconfig");
        sync.AddSourceItem(TreeEntryTargetType.Blob, "src/RepoSync/Source/FUNDING.yml", ".github/FUNDING.yml");
        sync.AddTargetRepository("Fody", "Fody", "master");
        sync.AddTargetRepository("Fody", "Anotar", "master");
        sync.AddTargetRepository("Fody", "AsyncErrorHandler", "master");
        sync.AddTargetRepository("Fody", "BasicFodyAddin", "master");
        sync.AddTargetRepository("Fody", "Caseless", "master");
        sync.AddTargetRepository("Fody", "ConfigureAwait", "master");
        sync.AddTargetRepository("Fody", "Costura", "master");
        sync.AddTargetRepository("Fody", "EmptyConstructor", "master");
        sync.AddTargetRepository("Fody", "Equals", "master");
        sync.AddTargetRepository("Fody", "ExtraConstraints", "master");
        sync.AddTargetRepository("Fody", "InfoOf", "master");
        sync.AddTargetRepository("Fody", "Ionad", "master");
        sync.AddTargetRepository("Fody", "Janitor", "master");
        sync.AddTargetRepository("Fody", "LoadAssembliesOnStartup", "master");
        sync.AddTargetRepository("Fody", "MethodDecorator", "master");
        sync.AddTargetRepository("Fody", "MethodTimer", "master");
        sync.AddTargetRepository("Fody", "ModuleInit", "master");
        sync.AddTargetRepository("Fody", "NullGuard", "master");
        sync.AddTargetRepository("Fody", "Obsolete", "master");
        sync.AddTargetRepository("Fody", "PropertyChanged", "master");
        sync.AddTargetRepository("Fody", "PropertyChanging", "master");
        sync.AddTargetRepository("Fody", "Publicize", "master");
        sync.AddTargetRepository("Fody", "Resourcer", "master");
        sync.AddTargetRepository("Fody", "Scalpel", "master");
        sync.AddTargetRepository("Fody", "ToString", "master");
        sync.AddTargetRepository("Fody", "Validar", "master");
        sync.AddTargetRepository("Fody", "Virtuosity", "master");
        sync.AddTargetRepository("Fody", "Visualize", "master");

        return sync.Sync(SyncOutput.MergePullRequest);
    }
}