using KRT.MaterialTools.Common;
using UnityEditor;
using UnityEngine;

namespace KRT.MaterialTools.TextureReplacer
{
    internal static class TextureReplacerMenu
    {
        [MenuItem(MenuEntry.MenuBar.TextureReplacer, priority = (int)MenuEntry.MenuBar.Priority.TextureReplacer)]
        private static void MenuBar()
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

        [MenuItem(MenuEntry.AssetMenu.TextureReplacer, false)]
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

        [MenuItem(MenuEntry.AssetMenu.TextureReplacer, true)]
        private static bool AssetMenuValidate()
        {
            return Selection.activeObject is Material;
        }

        [MenuItem(MenuEntry.ContextMenu.TextureReplacer, priority = (int)MenuEntry.ContextMenu.Priority.TextureReplacer)]
        private static void ContextMenu(MenuCommand command)
        {
            var window = EditorWindow.GetWindow<TextureReplacerWindow>();
            Material mat = (Material)command.context;
            ShowWindow(mat);
        }


        private static void ShowWindow(Material mat = null)
        {
            var window = EditorWindow.GetWindow<TextureReplacerWindow>();
            if (mat != null)
            {
                window.material = mat;
            }
            window.Show();
        }
    }
}
