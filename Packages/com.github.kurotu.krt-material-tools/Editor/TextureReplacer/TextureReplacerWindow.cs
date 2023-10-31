using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KRT.MaterialTools.TextureReplacer
{
    internal class TextureReplacerWindow : EditorWindow
    {
        internal Material material;

        [SerializeField]
        private TextureReplacerRule adhocRule = new TextureReplacerRule();

        [SerializeField]
        internal Vector2 scrollPosition;

        [MenuItem("Tools/KRT Material Tools/Texture Replacer")]
        private static void ShowFromMenu()
        {
            var window = GetWindow<TextureReplacerWindow>();
            window.Show();
        }

        private void OnEnable()
        {
            titleContent.text = "Texture Replacer";
        }

        private void OnGUI()
        {
            material = (Material)EditorGUILayout.ObjectField("Material", material, typeof(Material), false);
            if (material == null)
            {
                return;
            }

            var texturePropertyPairs = GetTexturePropertyPairs(material);
            var textures = texturePropertyPairs.Select(p => p.texture).Where(t => t != null).Distinct().ToArray();
            Dictionary<Texture, string[]> props = textures.ToDictionary(t => t, t => texturePropertyPairs.Where(p => p.texture == t).Select(p => p.propertyName).ToArray());

            using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scroll.scrollPosition;
                foreach (var tex in textures)
                {
                    using (var horizontal = new EditorGUILayout.HorizontalScope())
                    {
                        using (var disable = new EditorGUI.DisabledGroupScope(true))
                        {
                            EditorGUILayout.ObjectField(tex, tex.GetType(), false, GUILayout.Width(80), GUILayout.Height(80));
                        }

                        if (!adhocRule.ContainsKey(tex))
                        {
                            adhocRule.SetTexture(tex, null);
                        }
                        var replacement = adhocRule.GetTexture(tex);
                        adhocRule.SetTexture(tex, (Texture)EditorGUILayout.ObjectField(replacement, replacement != null ? replacement.GetType() : typeof(Texture), false, GUILayout.Width(80), GUILayout.Height(80)));

                        using (var vertical = new EditorGUILayout.VerticalScope())
                        {
                            foreach (var prop in props[tex])
                            {
                                EditorGUILayout.LabelField(prop, GUILayout.MinWidth(80));
                            }
                        }
                    }
                }
            }

            EditorGUILayout.Space();

            using (var horizontal = new EditorGUILayout.HorizontalScope())
            {
                using (var disable = new EditorGUI.DisabledGroupScope(adhocRule.IsBlank))
                {
                    if (GUILayout.Button("Apply"))
                    {
                        OnClickApply();
                    }

                    if (GUILayout.Button("Save As..."))
                    {
                        OnClickSaveAsNewMaterial();
                    }
                }
            }

            EditorGUILayout.Space();
        }

        private void OnClickApply()
        {
            ApplyTextures(material, adhocRule);
            adhocRule.Clear();
        }

        private void OnClickSaveAsNewMaterial()
        {
            var path = AssetDatabase.GetAssetPath(material);
            var newPath = AssetDatabase.GenerateUniqueAssetPath(path);
            newPath = EditorUtility.SaveFilePanelInProject("Save as New Material", Path.GetFileNameWithoutExtension(newPath), "mat", "", Application.dataPath);
            if (newPath == null)
            {
                return;
            }
            var newMat = new Material(material);
            ApplyTextures(newMat, adhocRule);
            AssetDatabase.CreateAsset(newMat, newPath);
            adhocRule.Clear();
        }

        private List<TexturePropertyPair> GetTexturePropertyPairs(Material material)
        {
            var list = new List<TexturePropertyPair>();
            var propNames = material.GetTexturePropertyNames();
            foreach (var prop in propNames)
            {
                var tex = material.GetTexture(prop);
                list.Add(new TexturePropertyPair { propertyName = prop, texture = tex });
            }
            return list;
        }

        private void ApplyTextures(Material mat, TextureReplacerRule rule)
        {
            Undo.RecordObject(mat, "Apply Textures");
            var texturePropertyPairs = GetTexturePropertyPairs(mat);
            foreach (var pair in rule.ToDictionary().Where(p => p.Value != null))
            {
                foreach (var prop in texturePropertyPairs.Where(tp => tp.texture == pair.Key))
                {
                    mat.SetTexture(prop.propertyName, pair.Value);
                }
            }
            Undo.FlushUndoRecordObjects();
        }

        private class TexturePropertyPair
        {
            public string propertyName;
            public Texture texture;
        }
    }
}
