using KRT.MaterialTools.Common;
using UnityEditor;
using UnityEngine;

namespace KRT.MaterialTools.QuickVariant
{
    internal static class QuickVariantMenu
    {
        [MenuItem(MenuEntry.MenuBar.QuickVariant, priority = (int)MenuEntry.MenuBar.Priority.QuickVariant)]
        private static void MenuBar()
        {
            ShowWindow(Selection.activeGameObject);
        }

        [MenuItem(MenuEntry.GameObjectMenu.QuickVariant, false, (int)MenuEntry.GameObjectMenu.Priority.QuickVariant)]
        private static void GameObjectMenu()
        {
            ShowWindow(Selection.activeGameObject);
        }

        [MenuItem(MenuEntry.GameObjectMenu.QuickVariant, true)]
        private static bool GameObjectMenuValidate()
        {
            return Selection.activeGameObject != null;
        }

        private static void ShowWindow(GameObject target)
        {
            var window = EditorWindow.GetWindow<QuickVariantWindow>();
            var changed = window.Target != Selection.activeGameObject;
            if (Selection.activeGameObject && changed)
            {
                window.SetTarget(Selection.activeGameObject);
            }
            window.Show();
        }
    }
}
