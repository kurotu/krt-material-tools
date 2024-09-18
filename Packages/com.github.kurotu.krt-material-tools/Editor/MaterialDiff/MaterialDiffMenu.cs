using KRT.MaterialTools.Common;
using UnityEditor;
using UnityEngine;

namespace KRT.MaterialTools.MaterialDiff
{
    internal static class MaterialDiffMenu
    {
        [MenuItem(MenuEntry.MenuBar.MaterialDiff, priority = (int)MenuEntry.MenuBar.Priority.MaterialDiff)]

        private static void MenuBar()
        {
            ShowWindow();
        }

        [MenuItem(MenuEntry.AssetMenu.MaterialDiff, false)]
        private static void AssetMenu()
        {
            if (Selection.activeObject is Material material)
            {
                ShowWindow(material);
            }
            else
            {
                ShowWindow();
            }
        }

        [MenuItem(MenuEntry.AssetMenu.MaterialDiff, true)]
        private static bool AssetMenuValidate()
        {
            return Selection.activeObject is Material;
        }


        [MenuItem(MenuEntry.ContextMenu.MaterialDiff, priority = (int)MenuEntry.ContextMenu.Priority.MaterialDiff)]
        private static void ContextMenu(MenuCommand command)
        {
            ShowWindow((Material)command.context);
        }

        private static void ShowWindow(Material mat = null)
        {
            var window = EditorWindow.GetWindow<MaterialDiffWindow>();
            if (mat != null)
            {
                window.leftMat = mat;
            }
            window.Show();
        }
    }
}
