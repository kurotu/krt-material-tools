using KRT.MaterialTools.Common;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KRT.MaterialTools.MaterialVariant
{
    internal static class MaterialVariantMenu
    {
        [MenuItem(MenuEntry.MenuBar.MaterialVariant, priority = (int)MenuEntry.MenuBar.Priority.MaterialVariant)]
        private static void MenuBar()
        {
            ShowWindow();
        }

        [MenuItem(MenuEntry.AssetMenu.MaterialVariant)]
        private static void AssetMenu()
        {
            var materials = Selection.objects.Where(o => o is Material).Cast<Material>().ToArray();
            ShowWindow(materials);
        }

        [MenuItem(MenuEntry.AssetMenu.MaterialVariant, validate = true)]
        private static bool ValidateAssetMenu()
        {
            return Selection.objects.Where(o => o is Material).Count() > 0;
        }

        private static void ShowWindow(Material[] materials = null)
        {
            var window = EditorWindow.GetWindow<MaterialVariantWindow>();
            if (materials != null)
            {
                window.Materials = materials.ToList();
            }
            window.Show();
        }
    }
}
