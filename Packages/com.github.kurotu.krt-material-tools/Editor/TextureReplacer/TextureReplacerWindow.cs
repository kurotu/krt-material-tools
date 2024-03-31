using System.Collections.Generic;
using System.IO;
using System.Linq;
using KRT.MaterialTools.Common;
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

        private const int TextureFieldWidth = 100;
        private const int PropertyLabelMinWidth = 80;

        private void OnEnable()
        {
            titleContent.text = "Texture Replacer";
        }

        private void OnGUI()
        {
            if (UpdateChecker.ShouldNotifyUpdate())
            {
                UpdateChecker.NotifyUpdateGUI();
                EditorGUILayout.Space();
            }

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                material = (Material)EditorGUILayout.ObjectField("Material", material, typeof(Material), false);
                if (scope.changed)
                {
                    adhocRule.Clear();
                }
            }

            if (material == null)
            {
                EditorGUILayout.HelpBox("Please select a material.", MessageType.Info);
                return;
            }

            var texturePropertyDict = GetTexturePropertyDictionary(material);
            var textures = texturePropertyDict.Select(p => p.Value).Where(t => t != null).Distinct().ToArray();
            Dictionary<Texture, string[]> props = textures.ToDictionary(t => t, t => texturePropertyDict.Where(p => p.Value == t).Select(p => p.Key).ToArray());

            EditorGUILayout.Space();

            using (var horizontal = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Original", EditorStyles.boldLabel, GUILayout.Width(TextureFieldWidth));
                EditorGUILayout.LabelField("Replacement", EditorStyles.boldLabel, GUILayout.Width(TextureFieldWidth));
                EditorGUILayout.LabelField("Properties", EditorStyles.boldLabel, GUILayout.MinWidth(PropertyLabelMinWidth));
            }

            using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scroll.scrollPosition;
                foreach (var tex in textures)
                {
                    using (var horizontal = new EditorGUILayout.HorizontalScope())
                    {
                        using (var disable = new EditorGUI.DisabledGroupScope(true))
                        {
                            EditorGUILayout.ObjectField(tex, tex.GetType(), false, GUILayout.Width(TextureFieldWidth), GUILayout.Height(TextureFieldWidth));
                        }

                        if (!adhocRule.ContainsKey(tex))
                        {
                            adhocRule.SetTexture(tex, null);
                        }
                        var replacement = adhocRule.GetTexture(tex);
                        adhocRule.SetTexture(tex, (Texture)EditorGUILayout.ObjectField(replacement, replacement != null ? replacement.GetType() : typeof(Texture), false, GUILayout.Width(TextureFieldWidth), GUILayout.Height(TextureFieldWidth)));

                        using (var vertical = new EditorGUILayout.VerticalScope())
                        {
                            foreach (var prop in props[tex])
                            {
                                EditorGUILayout.LabelField(prop, GUILayout.MinWidth(PropertyLabelMinWidth));
                            }
                            EditorGUILayout.Space();
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

        private Dictionary<string, Texture> GetTexturePropertyDictionary(Material material)
        {
            var dict = new Dictionary<string, Texture>();
            var propNames = material.GetTexturePropertyNames();
            foreach (var prop in propNames)
            {
                var tex = material.GetTexture(prop);
                dict[prop] = tex;
            }
            return dict;
        }

        private void ApplyTextures(Material mat, TextureReplacerRule rule)
        {
            Undo.RecordObject(mat, "Apply Textures");
            var texturePropertyDict = GetTexturePropertyDictionary(mat);
            foreach (var pair in rule.ToDictionary().Where(p => p.Value != null))
            {
                foreach (var prop in texturePropertyDict.Where(tp => tp.Value == pair.Key))
                {
                    mat.SetTexture(prop.Key, pair.Value);
                }
            }
            Undo.FlushUndoRecordObjects();
        }
    }
}
