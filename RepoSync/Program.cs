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
        var sync = new RepoSync(credentials, "Fody", "Fody", "master", Console.WriteLine);
        //sync.AddSourceItem(TreeEntryTargetType.Blob, "RepoSync/appveyor.yml", "appveyor.yml");
        sync.AddSourceItem(TreeEntryTargetType.Blob, ".editorconfig", ".editorconfig");
        sync.AddSourceItem(TreeEntryTargetType.Blob, ".github/ISSUE_TEMPLATE/bug_report.md", ".github/ISSUE_TEMPLATE/bug_report.md");
        sync.AddSourceItem(TreeEntryTargetType.Blob, ".github/ISSUE_TEMPLATE/feature_proposal.md", ".github/ISSUE_TEMPLATE/feature_proposal.md");
        sync.AddSourceItem(TreeEntryTargetType.Blob, ".github/pull_request_template.md", ".github/pull_request_template.md");
        sync.AddTarget("Fody", "Anotar", "master");
        sync.AddTarget("Fody", "AsyncErrorHandler", "master");
        sync.AddTarget("Fody", "BasicFodyAddin", "master");
        sync.AddTarget("Fody", "Caseless", "master");
        sync.AddTarget("Fody", "ConfigureAwait", "master");
        sync.AddTarget("Fody", "Costura", "master");
        sync.AddTarget("Fody", "EmptyConstructor", "master");
        sync.AddTarget("Fody", "Equals", "master");
        sync.AddTarget("Fody", "ExtraConstraints", "master");
        sync.AddTarget("Fody", "Fielder", "master");
        sync.AddTarget("Fody", "Freezable", "master");
        sync.AddTarget("Fody", "Immutable", "master");
        sync.AddTarget("Fody", "InfoOf", "master");
        sync.AddTarget("Fody", "Ionad", "master");
        sync.AddTarget("Fody", "Janitor", "master");
        sync.AddTarget("Fody", "LoadAssembliesOnStartup", "master");
        sync.AddTarget("Fody", "MethodDecorator", "master");
        sync.AddTarget("Fody", "MethodTimer", "master");
        sync.AddTarget("Fody", "ModuleInit", "master");
        sync.AddTarget("Fody", "NullGuard", "master");
        sync.AddTarget("Fody", "Obsolete", "master");
        sync.AddTarget("Fody", "PropertyChanged", "master");
        sync.AddTarget("Fody", "PropertyChanging", "master");
        sync.AddTarget("Fody", "Publicize", "master");
        sync.AddTarget("Fody", "Resourcer", "master");
        sync.AddTarget("Fody", "Scalpel", "master");
        sync.AddTarget("Fody", "ToString", "master");
        sync.AddTarget("Fody", "Unsealed", "master");
        sync.AddTarget("Fody", "Usable", "master");
        sync.AddTarget("Fody", "Validar", "master");
        sync.AddTarget("Fody", "Virtuosity", "master");
        sync.AddTarget("Fody", "Visualize", "master");

        return sync.Sync(SyncOutput.MergePullRequest);
    }
}