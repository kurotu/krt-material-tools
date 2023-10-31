using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace KRT.MaterialTools.Common
{
    internal static class UpdateChecker
    {
        /// <summary>
        /// Version of MaterialReplacer.
        /// </summary>
        public static readonly string Version = "0.0.0";

        /// <summary>
        /// URL for GitHub.
        /// </summary>
        internal const string GitHubURL = "https://github.com/" + Repository;

        /// <summary>
        /// Latest release info.
        /// </summary>
        internal static GitHubRelease latestRelease = null;

        private const string PackageJsonGUID = "87dd96c0da3590f4aa8a28cfb43bee61";

        private const string Repository = "kurotu/krt-material-tools";

        private static readonly HttpClient Client = new HttpClient();

        static UpdateChecker()
        {
            Client.Timeout = TimeSpan.FromSeconds(10);
            Client.DefaultRequestHeaders.Add("User-Agent", $"KRT Material Tools {Version}");

            Task.Run(async () =>
            {
                try
                {
                    latestRelease = await GetLatestRelease();
                    if (latestRelease != null && latestRelease.Version >= new SemVer(Version))
                    {
                        Debug.LogWarning($"New Version {latestRelease.Version} is available.");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    latestRelease = null;
                }
            });
        }

        private static string AssetRoot => Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(PackageJsonGUID));

        /// <summary>
        /// Gets whether update should be notified to user.
        /// </summary>
        /// <returns>true to show notification.</returns>
        internal static bool ShouldNotifyUpdate()
        {
            var current = new SemVer(Version);
            if (latestRelease == null)
            {
                return false;
            }
            if (latestRelease.Version <= current)
            {
                return false;
            }
            if (!current.IsPreRelease && latestRelease.Version.IsPreRelease)
            {
                return false;
            }

            var span = DateTime.UtcNow - latestRelease.PublishedDateTime;
            return span.TotalDays > 1;
        }

        internal static void NotifyUpdateGUI()
        {
            using (var box = new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                var color = GUI.contentColor;
                GUI.contentColor = Color.red;
                EditorGUILayout.LabelField($"Update: {Version} -> {latestRelease.Version}", EditorStyles.boldLabel);
                GUI.contentColor = color;
                using (var horizontal = new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("GitHub"))
                    {
                        Application.OpenURL(GitHubURL);
                    }
                }
            }
        }

        private static async Task<GitHubRelease> GetLatestRelease()
        {
            var url = $"https://api.github.com/repos/{Repository}/releases/latest";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/vnd.github.v3+json");
            var response = await Client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError($"Failed {request.Method} {request.RequestUri}: {(int)response.StatusCode} {response.ReasonPhrase}");
                return null;
            }
            var body = await response.Content.ReadAsStringAsync();
            var release = JsonUtility.FromJson<GitHubRelease>(body);
            return release;
        }
    }
}
