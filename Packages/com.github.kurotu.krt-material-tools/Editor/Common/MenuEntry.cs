namespace KRT.MaterialTools.Common
{
    internal static class MenuEntry
    {
        private const string Name = "KRT Material Tools";
        private const string MenuBarRoot = "Tools/" + Name;

        internal static class MenuBar
        {
            internal const string MaterialDiff = MenuBarRoot + "/Material Diff";
            internal const string MaterialReplacer = MenuBarRoot + "/Material Replacer";
            internal const string MaterialVariant = MenuBarRoot + "/Material Variant";
            internal const string QuickVariant = MenuBarRoot + "/Quick Variant";
            internal const string TextureReplacer = MenuBarRoot + "/Texture Replacer";
            internal const string MigrateLegacyAssets = MenuBarRoot + "/Migrate Legacy Assets";

            internal enum Priority
            {
                MaterialDiff = 610, // VRCQuestTools: 600
                MaterialReplacer,
                MaterialVariant,
                QuickVariant,
                TextureReplacer,
                MigrateLegacyAssets = 630,
            }
        }

        internal static class AssetMenu
        {
            private const string AssetMenuRoot = "Assets/" + Name;
            internal const string MaterialDiff = AssetMenuRoot + "/Material Diff";
            internal const string MaterialVariant = AssetMenuRoot + "/Material Variant";
            internal const string TextureReplacer = AssetMenuRoot + "/Texture Replacer";
        }

        internal static class CreateAssetMenu
        {
            internal const string MaterialReplacerRule = Name + "/Material Replacer Rule";

            internal enum Priority
            {
                MaterialReplacerRule = 1000,
            }
        }

        internal static class ContextMenu
        {
            internal const string MaterialDiff = "CONTEXT/Material/" + Name + "/Material Diff";
            internal const string TextureReplacer = "CONTEXT/Material/" + Name + "/Texture Replacer";

            internal enum Priority
            {
                MaterialDiff = 1100,
                TextureReplacer,
            }
        }

        internal static class GameObjectMenu
        {
            internal const string MaterialReplacer = "GameObject/" + Name + "/Material Replacer";
            internal const string QuickVariant = "GameObject/" + Name + "/Quick Variant";

            internal enum Priority
            {
                MaterialReplacer = 30,
                QuickVariant,
            }
        }
    }
}
