using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KRT.MaterialTools.TextureReplacer
{
    internal class TextureReplacerWindow : EditorWindow
    {
        internal Material material;
        internal Dictionary<Texture, Texture> adhocRule = new Dictionary<Texture, Texture>();

        private List<TexturePropertyPair> texturePairs;

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

            texturePairs = GetTextures(material);
            var textures = texturePairs.Select(p => p.texture).Where(t => t != null).Distinct().ToArray();
            Dictionary<Texture, string[]> props = textures.ToDictionary(t => t, t => texturePairs.Where(p => p.texture == t).Select(p => p.propertyName).ToArray());

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
                            adhocRule[tex] = null; 
                        }
                        var replacement = adhocRule[tex];
                        adhocRule[tex] = (Texture)EditorGUILayout.ObjectField(replacement, replacement != null ? replacement.GetType() : typeof(Texture), false, GUILayout.Width(80), GUILayout.Height(80));

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

            if (GUILayout.Button("Apply"))
            {
                OnClickApply();
            }

            EditorGUILayout.Space();
        }

        private void OnClickApply()
        {
            adhocRule.Where(p => p.Value != null).ToList().ForEach(p =>
            {
                foreach (var prop in texturePairs.Where(tp => tp.texture == p.Key))
                {
                    material.SetTexture(prop.propertyName, p.Value);
                }
            });
        }

        private List<TexturePropertyPair> GetTextures(Material material)
        {
            var list = new List<TexturePropertyPair>();
            var propNames = material.GetTexturePropertyNames();
            foreach(var prop in propNames)
            {
                var tex = material.GetTexture(prop);
                list.Add(new TexturePropertyPair { propertyName = prop, texture = tex });
            }
            return list;
        }

        [Serializable]
        internal class TexturePair
        {
            public Texture original;
            public Texture replacement;
        }

        private class TexturePropertyPair
        {
            public string propertyName;
            public Texture texture;
        }
    }
}
