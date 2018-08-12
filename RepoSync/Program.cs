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
        var repoSync = new RepoSync(credentials, "Fody", "Fody", "master", Console.WriteLine);
        repoSync.AddSourceItem(TreeEntryTargetType.Blob, "RepoSync/appveyor.yml", "appveyor.yml");
        repoSync.AddTarget("Fody", "Caseless", "master");

        return repoSync.Sync();
    }
}