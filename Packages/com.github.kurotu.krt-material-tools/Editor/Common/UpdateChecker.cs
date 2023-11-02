using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace KRT.MaterialTools.Common
{
    internal static class UpdateChecker
    {
        /// <summary>
        /// Version of MaterialReplacer.
        /// </summary>
        public static readonly string Version = "0.0.1-alpha.1";

        /// <summary>
        /// URL for GitHub.
        /// </summary>
        internal const string GitHubURL = "https://github.com/" + Repository;

        internal const string VpmRepoURL = "https://kurotu.github.io/krt-material-tools/index.json";
        internal const string VpmPackageName = "com.github.kurotu.krt-material-tools";

        /// <summary>
        /// Latest release info.
        /// </summary>
        internal static SemVer latestRelease = null;

        private const string Repository = "kurotu/krt-material-tools";

        private static readonly HttpClient Client = new HttpClient();

        static UpdateChecker()
        {
            Client.Timeout = System.TimeSpan.FromSeconds(10);
            Client.DefaultRequestHeaders.Add("User-Agent", $"KRT Material Tools {Version}");

            Task.Run(async () =>
            {
                try
                {
                    var current = new SemVer(Version);
                    latestRelease = await GetLatestRelease(current.IsPreRelease);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogError("Failed to get latest release version.");
                    latestRelease = null;
                }
                if (ShouldNotifyUpdate())
                {
                    Debug.LogWarning($"New Version {latestRelease} is available.");
                }
            });
        }

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
            if (latestRelease <= current)
            {
                return false;
            }
            if (!current.IsPreRelease && latestRelease.IsPreRelease)
            {
                return false;
            }

            return true;
        }

        internal static void NotifyUpdateGUI()
        {
            using (var box = new EditorGUILayout.HorizontalScope(GUI.skin.box))
            {
                var color = GUI.contentColor;
                GUI.contentColor = Color.red;
                EditorGUILayout.LabelField($"Update: {Version} -> {latestRelease}", EditorStyles.boldLabel);
                GUI.contentColor = color;

                if (GUILayout.Button("GitHub", GUILayout.MaxWidth(80)))
                {
                    Application.OpenURL($"{GitHubURL}/releases/{latestRelease}");
                }
            }
        }

        private static async Task<SemVer> GetLatestRelease(bool allowPrerRelease)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, VpmRepoURL);
            var response = await Client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError($"Failed {request.Method} {request.RequestUri}: {(int)response.StatusCode} {response.ReasonPhrase}");
                return null;
            }
            var body = await response.Content.ReadAsStringAsync();

            var repo = JObject.Parse(body);
            var versions = repo.GetValue("packages").ToObject<JObject>()
                .GetValue(VpmPackageName).ToObject<JObject>()
                .GetValue("versions").ToObject<JObject>()
                .Properties().Select(p => p.Name)
                .Select(v => new SemVer(v)).ToList();

            if (!allowPrerRelease)
            {
                versions = versions.Where(v => !v.IsPreRelease).ToList();
            }

            if (versions.Count == 0)
            {
                return null;
            }

            versions.Sort();
            return versions.Last();
        }
    }
}
