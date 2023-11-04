using System.IO;
using System.Linq;
using KRT.MaterialTools.Common;
using UnityEditor;
using UnityEngine;

namespace KRT.MaterialTools.MaterialReplacer
{
    internal static class MaterialReplacerRuleMigrator
    {
        private const string LegacyGUID = "a9040729468841e45aa2294ceae6ae2a";
        private static readonly string NewGUID;

        private static readonly ILogger logger = InternalLogger.Logger;
        private const string TAG = "MaterialReplacerRuleMigrator";

        static MaterialReplacerRuleMigrator()
        {
            var script = MonoScript.FromScriptableObject(ScriptableObject.CreateInstance<MaterialReplacerRule>());
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(script, out var guid, out long _);
            NewGUID = guid;
        }

        internal static void MigrateLegacyAssets()
        {
            var assetPaths = AssetDatabase.GetAllAssetPaths()
                .Where(p => p.StartsWith("Assets/") && p.EndsWith(".asset"))
                .ToArray();

            var counter = 0;
            foreach (var assetPath in assetPaths)
            {
                var text = File.ReadAllText(assetPath);
                if (!text.Contains($"guid: {LegacyGUID}"))
                {
                    continue;
                }
                logger.Log(TAG, $"Migrating {assetPath}");
                text = ReplaceScriptGUID(text, LegacyGUID, NewGUID);
                File.WriteAllText(assetPath, text);
                counter++;
            }
            if (counter > 0)
            {
                AssetDatabase.Refresh();
                var message = $"Migrated {counter} assets.";
                logger.Log(TAG, message);
                EditorUtility.DisplayDialog("Migrate Legacy Assets - KRT Material Tools", message, "OK");
            }
            else
            {
                const string message = "No assets to migrate in Assets/ folder.";
                logger.Log(TAG, message);
                EditorUtility.DisplayDialog("Migrate Legacy Assets - KRT Material Tools", message, "OK");
            }
        }

        private static string ReplaceScriptGUID(string assetText, string oldGUID, string newGUID)
        {
            return assetText.Replace($"m_Script: {{fileID: 11500000, guid: {oldGUID}, type: 3}}", $"m_Script: {{fileID: 11500000, guid: {newGUID}, type: 3}}");
        }
    }
}
