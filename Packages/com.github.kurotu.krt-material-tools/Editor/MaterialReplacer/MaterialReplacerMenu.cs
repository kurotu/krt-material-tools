using KRT.MaterialTools.Common;
using UnityEditor;

namespace KRT.MaterialTools.MaterialReplacer
{
    static class MaterialReplacerMenu
    {
        [MenuItem(MenuEntry.MenuBar.MaterialReplacer, priority = (int)MenuEntry.MenuBar.Priority.MaterialReplacer)]
        [MenuItem(MenuEntry.GameObjectMenu.MaterialReplacer, false, (int)MenuEntry.GameObjectMenu.Priority.MaterialReplacer)]
        private static void ShowFromMenu()
        {
            var window = EditorWindow.GetWindow<MaterialReplacerWindow>();
            if (Selection.activeGameObject)
            {
                window.Show(Selection.activeGameObject);
            }
            else
            {
                window.Show();
            }
        }

        [MenuItem(MenuEntry.MenuBar.MigrateLegacyAssets, priority = (int)MenuEntry.MenuBar.Priority.MigrateLegacyAssets)]
        private static void MigrateAssets()
        {
            MaterialReplacerRuleMigrator.MigrateLegacyAssets();
        }

        [MenuItem(MenuEntry.GameObjectMenu.MaterialReplacer, true)]
        private static bool ShowFromMenuValidate()
        {
            return Selection.activeGameObject != null;
        }
    }
}
