using System;

namespace GitFlow
{
    public enum GitHost { GitHub, AzureDevOps, Bitbucket, Unknown }

    public static class GitHostDetector
    {
        public static GitHost DetectHost(string remoteUrl)
        {
            if (string.IsNullOrWhiteSpace(remoteUrl))
                return GitHost.Unknown;

            var url = remoteUrl.ToLowerInvariant();

            if (url.Contains("github.com"))
                return GitHost.GitHub;
            if (url.Contains("dev.azure.com") || url.Contains("visualstudio.com"))
                return GitHost.AzureDevOps;
            if (url.Contains("bitbucket.org"))
                return GitHost.Bitbucket;

            return GitHost.Unknown;
        }

        public static string GetTokenCreationUrl(GitHost host)
        {
            switch (host)
            {
                case GitHost.GitHub:
                    return "https://github.com/settings/tokens";
                case GitHost.AzureDevOps:
                    return "https://dev.azure.com/_usersSettings/tokens";
                case GitHost.Bitbucket:
                    return "https://bitbucket.org/account/settings/app-passwords/";
                default:
                    return "";
            }
        }

        public static string GetHostDisplayName(GitHost host)
        {
            switch (host)
            {
                case GitHost.GitHub: return "GitHub";
                case GitHost.AzureDevOps: return "Azure DevOps";
                case GitHost.Bitbucket: return "Bitbucket";
                default: return "Git";
            }
        }
    }
}
