namespace KRT.MaterialTools.Common
{
    internal static class MenuEntry
    {
        private const string Name = "KRT Material Tools";
        private const string MenuBarRoot = "Tools/" + Name;

        internal static class MenuBar
        {
            internal const string MaterialReplacer = MenuBarRoot + "/Material Replacer";
            internal const string TextureReplacer = MenuBarRoot + "/Texture Replacer";

            internal enum Priority
            {
                MaterialReplacer = 610, // VRCQuestTools: 600
                TextureReplacer,
            }
        }

        internal static class AssetMenu
        {
            internal const string TextureReplacer = "Assets/" + Name + "/Texture Replacer";
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
            internal const string TextureReplacer = "CONTEXT/Material/" + Name + "/Texture Replacer";

            internal enum Priority
            {
                TextureReplacer = 1100,
            }
        }

        internal static class GameObjectMenu
        {
            internal const string MaterialReplacer = "GameObject/" + Name + "/Material Replacer";

            internal enum Priority
            {
                MaterialReplacer = 30,
            }
        }
    }
}
