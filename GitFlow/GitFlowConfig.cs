using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using Meziantou.Framework.Win32;

namespace GitFlow
{
    /// <summary>
    /// Persistent configuration stored in the GitFlow UserExtensions folder.
    /// Stores non-secret settings (username, email) per host.
    /// PATs are stored securely in Windows Credential Manager keyed by host.
    /// </summary>
    [DataContract]
    public class GitFlowConfig
    {
        private static GitFlowConfig _instance;
        private static string _configPath;

        [DataContract]
        public class HostConfig
        {
            [DataMember] public string Username { get; set; } = "";
            [DataMember] public string Email { get; set; } = "";
        }

        [DataMember]
        public Dictionary<string, HostConfig> Hosts { get; set; } = new Dictionary<string, HostConfig>();

        /// <summary>
        /// Gets the credential key used in Windows Credential Manager for a given host.
        /// </summary>
        public static string GetCredentialKey(string host)
        {
            return $"GitFlow:{host}";
        }

        /// <summary>
        /// Extracts the host from a remote URL (e.g. "github.com" from "https://github.com/user/repo.git").
        /// </summary>
        public static string ExtractHost(string remoteUrl)
        {
            if (string.IsNullOrEmpty(remoteUrl))
                return "";

            try
            {
                // Handle https:// URLs
                if (remoteUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    var uri = new Uri(remoteUrl);
                    return uri.Host.ToLowerInvariant();
                }

                // Handle SSH URLs like git@github.com:user/repo.git
                if (remoteUrl.Contains("@") && remoteUrl.Contains(":"))
                {
                    var afterAt = remoteUrl.Substring(remoteUrl.IndexOf('@') + 1);
                    var host = afterAt.Substring(0, afterAt.IndexOf(':'));
                    return host.ToLowerInvariant();
                }
            }
            catch { }

            return "";
        }

        /// <summary>
        /// Saves a PAT for a host in Windows Credential Manager, and username/email in config.
        /// </summary>
        public static void SaveHostCredential(string remoteUrl, string username, string pat, string email)
        {
            string host = ExtractHost(remoteUrl);
            if (string.IsNullOrEmpty(host)) return;

            // Save PAT securely in Credential Manager
            if (!string.IsNullOrEmpty(pat))
            {
                string key = GetCredentialKey(host);
                CredentialManager.WriteCredential(
                    applicationName: key,
                    userName: username,
                    secret: pat,
                    comment: email,
                    persistence: CredentialPersistence.LocalMachine);
            }

            // Save username/email in config file
            var config = Load();
            config.Hosts[host] = new HostConfig { Username = username, Email = email };
            config.Save();
        }

        /// <summary>
        /// Reads the stored PAT for a host from Windows Credential Manager.
        /// Returns null if no credential found.
        /// </summary>
        public static Credential ReadHostCredential(string remoteUrl)
        {
            string host = ExtractHost(remoteUrl);
            if (string.IsNullOrEmpty(host)) return null;

            return CredentialManager.ReadCredential(GetCredentialKey(host));
        }

        /// <summary>
        /// Gets the stored username/email for a host from config.
        /// </summary>
        public static HostConfig GetHostConfig(string remoteUrl)
        {
            string host = ExtractHost(remoteUrl);
            if (string.IsNullOrEmpty(host)) return null;

            var config = Load();
            config.Hosts.TryGetValue(host, out var hostConfig);
            return hostConfig;
        }

        /// <summary>
        /// Loads the config from disk, or returns a new empty config.
        /// </summary>
        public static GitFlowConfig Load()
        {
            if (_instance != null) return _instance;

            string path = GetConfigPath();
            if (File.Exists(path))
            {
                try
                {
                    using (var stream = File.OpenRead(path))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(GitFlowConfig));
                        _instance = (GitFlowConfig)serializer.ReadObject(stream);
                        return _instance;
                    }
                }
                catch { }
            }

            _instance = new GitFlowConfig();
            return _instance;
        }

        /// <summary>
        /// Saves the config to disk.
        /// </summary>
        public void Save()
        {
            string path = GetConfigPath();
            try
            {
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                using (var stream = File.Create(path))
                {
                    var serializer = new DataContractJsonSerializer(typeof(GitFlowConfig));
                    serializer.WriteObject(stream, this);
                }
            }
            catch { }
        }

        private static string GetConfigPath()
        {
            if (_configPath != null) return _configPath;

            // Look for the GitFlow folder in SimioUserExtensions
            string assemblyDir = Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);

            _configPath = Path.Combine(assemblyDir, "gitflow-config.json");
            return _configPath;
        }
    }
}
