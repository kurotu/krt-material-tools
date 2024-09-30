using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace KRT.MaterialTools.Common
{
    internal static class CommonUtil
    {
        internal static string SelectAssetDirectory(string title)
        {
            var directory = EditorUtility.SaveFolderPanel(title, "Assets", "");
            if (string.IsNullOrEmpty(directory))
            {
                return null;
            }
            directory = Path.GetRelativePath(Application.dataPath, directory);
            if (directory.StartsWith("."))
            {
                Debug.LogError("Folder must be in Assets/ folder");
                EditorUtility.DisplayDialog("Error", "Folder must be in Assets/ folder", "OK");
                return null;
            }
            directory = "Assets/" + directory;
            return directory;
        }

        internal static Dictionary<Material, Material> DuplicateMaterials(Material[] materials, string directory, string prefix, string suffix, bool asVariant)
        {
            var duplicatedMaterials = new Dictionary<Material, Material>();
            foreach (var material in materials)
            {
                var newMaterial = Object.Instantiate(material);
                newMaterial.name = $"{prefix}{material.name}{suffix}";
#if UNITY_2022_1_OR_NEWER
                if (asVariant)
                {
                    newMaterial.parent = material;
                }
#endif
                var filePath = $"{directory}/{newMaterial.name}.mat";
                filePath = AssetDatabase.GenerateUniqueAssetPath(filePath);
                AssetDatabase.CreateAsset(newMaterial, filePath);
                duplicatedMaterials.Add(material, newMaterial);
            }
            return duplicatedMaterials;
        }
    }
}
