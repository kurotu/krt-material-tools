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
            var materials = Selection.GetFiltered<Material>(SelectionMode.Assets);
            if (materials.Length == 0)
            {
                ShowWindow();
            }
            else if (materials.Length == 1)
            {
                ShowWindow(materials[0]);
            }
            else
            {
                ShowWindow(materials[0], materials[1]);
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

        private static void ShowWindow(Material leftMat = null, Material rightMat = null)
        {
            var window = EditorWindow.GetWindow<MaterialDiffWindow>();
            if (leftMat != null)
            {
                window.leftMat = leftMat;
            }
            if (rightMat != null)
            {
                window.rightMat = rightMat;
            }
            window.Show();
        }
    }
}
