using System.Collections.Generic;
using System.Linq;
using KRT.MaterialTools.Common;
using UnityEditor;
using UnityEngine;

namespace KRT.MaterialTools.MaterialReplacer
{
    /// <summary>
    /// Window to apply MaterialReplacerRule to renderers.
    /// </summary>
    internal class MaterialReplacerWindow : EditorWindow
    {
        [SerializeField]
        private GameObject targetObject;
        [SerializeField]
        private MaterialReplacerRule materialReplacerRule;
        private MaterialReplacerRule adhocRule;
        private Vector2 scrollPosition;

        internal void Show(GameObject targetObject)
        {
            this.targetObject = targetObject;
            Show();
        }

        private static Material[] GetRendererMaterials(GameObject gameObject)
        {
            var targetMaterials = gameObject.GetComponentsInChildren<Renderer>(true)
                .SelectMany(r => r.sharedMaterials)
                .Where(m => m != null)
                .Distinct()
                .ToArray();
            return targetMaterials;
        }

        private static Dictionary<Material, Material> ResolveRules(Material[] targetMaterials, IEnumerable<MaterialReplacerRule> rules)
        {
            var resolved = targetMaterials.ToDictionary(t => t, t =>
            {
                var chosenRule = rules
                    .Where(r => r != null)
                    .Reverse()
                    .FirstOrDefault(r => r.ContainsKey(t) && r[t] != null);
                if (chosenRule == null)
                {
                    return t;
                }
                return chosenRule[t];
            });
            return resolved;
        }

        private void OnEnable()
        {
            titleContent.text = "Material Replacer";
            if (adhocRule == null)
            {
                adhocRule = CreateInstance<MaterialReplacerRule>();
            }
        }

        private void OnGUI()
        {
            if (UpdateChecker.ShouldNotifyUpdate())
            {
                UpdateChecker.NotifyUpdateGUI();
                EditorGUILayout.Space();
            }

            targetObject = (GameObject)EditorGUILayout.ObjectField("Game Object", targetObject, typeof(GameObject), true);
            materialReplacerRule = (MaterialReplacerRule)EditorGUILayout.ObjectField("Material Replacer Rule", materialReplacerRule, typeof(MaterialReplacerRule), false);

            if (targetObject)
            {
                var targetMaterials = GetRendererMaterials(targetObject);
                var baseResults = materialReplacerRule ? targetMaterials.Select(t => materialReplacerRule.ContainsKey(t) ? materialReplacerRule[t] : null).ToArray() : null;
                var adhocResults = targetMaterials.Select(t => adhocRule.ContainsKey(t) ? adhocRule[t] : null).ToArray();
                var resolvedRule = ResolveRules(targetMaterials, new MaterialReplacerRule[] { materialReplacerRule, adhocRule });
                if (targetMaterials.Length > 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Green materials will be applied.");
                    using (var box = new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Original");
                        if (materialReplacerRule)
                        {
                            EditorGUILayout.LabelField(materialReplacerRule.name);
                        }
                        EditorGUILayout.LabelField("Ad Hoc Rule");
                    }

                    using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPosition))
                    {
                        var defaultBackground = GUI.backgroundColor;
                        var defaultContent = GUI.contentColor;
                        var activeColor = Color.green;
                        for (var i = 0; i < targetMaterials.Length; i++)
                        {
                            var resolved = resolvedRule[targetMaterials[i]];
                            if (resolved == null)
                            {
                                resolved = targetMaterials[i];
                            }
                            using (var box = new EditorGUILayout.HorizontalScope())
                            {
                                using (new EditorGUI.DisabledScope(true))
                                {
                                    GUI.backgroundColor = resolved == targetMaterials[i] ? activeColor : defaultBackground;
                                    GUI.contentColor = GUI.backgroundColor;
                                    EditorGUILayout.ObjectField(targetMaterials[i], typeof(Material), false);
                                    if (materialReplacerRule)
                                    {
                                        GUI.backgroundColor = resolved == baseResults[i] ? activeColor : defaultBackground;
                                        GUI.contentColor = GUI.backgroundColor;
                                        EditorGUILayout.ObjectField(baseResults[i], typeof(Material), false);
                                    }
                                }
                                GUI.backgroundColor = resolved == adhocResults[i] ? activeColor : defaultBackground;
                                GUI.contentColor = GUI.backgroundColor;
                                adhocRule[targetMaterials[i]] = (Material)EditorGUILayout.ObjectField(adhocResults[i], typeof(Material), false);
                            }
                        }
                        GUI.contentColor = defaultContent;
                        GUI.backgroundColor = defaultBackground;
                        scrollPosition = scroll.scrollPosition;
                    }
                }
            }

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledGroupScope(targetObject == null))
            {
                if (GUILayout.Button("Apply"))
                {
                    OnClickApply();
                }
            }
        }

        private void OnClickApply()
        {
            var resolvedRule = ResolveRules(GetRendererMaterials(targetObject), new MaterialReplacerRule[] { materialReplacerRule, adhocRule });
            var renderers = targetObject.GetComponentsInChildren<Renderer>(true);

            for (var rendererIndex = 0; rendererIndex < renderers.Length; rendererIndex++)
            {
                var renderer = renderers[rendererIndex];
                Undo.RecordObject(renderer, "Apply Material Replacer");
                renderer.sharedMaterials = renderer.sharedMaterials.Select(m =>
                {
                    if (m == null)
                    {
                        return null;
                    }
                    if (!resolvedRule.ContainsKey(m))
                    {
                        return m;
                    }
                    var resolved = resolvedRule[m];
                    if (resolved)
                    {
                        return resolved;
                    }
                    return m;
                }).ToArray();
            }
        }
    }
}
