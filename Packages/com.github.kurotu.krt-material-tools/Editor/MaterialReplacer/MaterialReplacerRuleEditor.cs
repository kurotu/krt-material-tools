using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace KRT.MaterialTools.MaterialReplacer
{
    /// <summary>
    /// Inspector for MaterialReplacerRule.
    /// </summary>
    [CustomEditor(typeof(MaterialReplacerRule))]
    internal class MaterialReplacerRuleEditor : Editor
    {
        private const int FIELD_MARGIN = 2;
        private const string PROPERTY_SERIALIZED_MATERIALS = "m_SerializedMaterials";
        private const string PROPERTY_ORIGINAL = "original";
        private const string PROPERTY_REPLACED = "replaced";

        private ReorderableList list;
        private GameObject referenceObject;

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            if (MaterialReplacer.ShouldNotifyUpdate())
            {
                using (var box = new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
                    var color = GUI.contentColor;
                    GUI.contentColor = Color.red;
                    EditorGUILayout.LabelField($"Update: {MaterialReplacer.Version} -> {MaterialReplacer.latestRelease.Version}", EditorStyles.boldLabel);
                    GUI.contentColor = color;
                    using (var horizontal = new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Booth"))
                        {
                            Application.OpenURL(MaterialReplacer.BoothURL);
                        }
                        if (GUILayout.Button("GitHub"))
                        {
                            Application.OpenURL(MaterialReplacer.GitHubURL);
                        }
                    }
                }
                EditorGUILayout.Space();
            }

            serializedObject.Update();
            list.DoLayoutList();

            if (HasCopiedOriginal())
            {
                EditorGUILayout.HelpBox("warning", MessageType.Warning);
            }

            EditorGUILayout.Space();

            referenceObject = (GameObject)EditorGUILayout.ObjectField("Reference Object", referenceObject, typeof(GameObject), true);
            if (referenceObject)
            {
                using (var box = new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
                    EditorGUILayout.LabelField("Materials in all renderers:", EditorStyles.boldLabel);
                    foreach (var material in GetMaterials(referenceObject))
                    {
                        if (GUILayout.Button($"- {material.name}", GUI.skin.label))
                        {
                            EditorGUIUtility.PingObject(material);
                        }
                    }
                }
                if (GUILayout.Button("Add to Original Materials"))
                {
                    OnClickAddToOriginals();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static Material[] GetMaterials(GameObject gameObject)
        {
            var materials = gameObject.GetComponentsInChildren<Renderer>(true).SelectMany(r => r.sharedMaterials);
            return materials.Where(m => m != null).Distinct().ToArray();
        }

        private void OnEnable()
        {
            var materials = serializedObject.FindProperty(PROPERTY_SERIALIZED_MATERIALS);
            list = new ReorderableList(serializedObject, materials, true, true, true, true);
            list.drawHeaderCallback += (Rect rect) =>
            {
                var left = new Rect(rect)
                {
                    width = rect.width / 2,
                };
                EditorGUI.LabelField(left, "Original");
                var right = new Rect(left)
                {
                    width = left.width,
                    x = rect.x + left.width,
                };
                EditorGUI.LabelField(right, "Replaced");
            };
            list.drawElementCallback += (Rect rect, int index, bool selected, bool focused) =>
            {
                var pair = materials.GetArrayElementAtIndex(index);
                var left = new Rect(rect)
                {
                    width = rect.width / 2 - FIELD_MARGIN,
                };
                var originalProperty = pair.FindPropertyRelative(PROPERTY_ORIGINAL);
                EditorGUI.PropertyField(left, originalProperty, new GUIContent(string.Empty));

                var right = new Rect(left)
                {
                    x = left.x + left.width + FIELD_MARGIN * 2,
                };
                var replacedProperty = pair.FindPropertyRelative(PROPERTY_REPLACED);
                EditorGUI.PropertyField(right, replacedProperty, new GUIContent(string.Empty));
            };
            list.onAddCallback += (list) =>
            {
                materials.arraySize += 1;
                var pair = materials.GetArrayElementAtIndex(materials.arraySize - 1);
                pair.FindPropertyRelative(PROPERTY_ORIGINAL).objectReferenceValue = null;
                pair.FindPropertyRelative(PROPERTY_REPLACED).objectReferenceValue = null;
            };
        }

        private void OnClickAddToOriginals()
        {
            var refMats = GetMaterials(referenceObject);
            var materials = serializedObject.FindProperty(PROPERTY_SERIALIZED_MATERIALS);
            var size = materials.arraySize;
            materials.arraySize += refMats.Length;
            for (var i = 0; i < refMats.Length; i++)
            {
                var pair = materials.GetArrayElementAtIndex(size + i);
                pair.FindPropertyRelative(PROPERTY_ORIGINAL).objectReferenceValue = refMats[i];
                pair.FindPropertyRelative(PROPERTY_REPLACED).objectReferenceValue = null;
            }
            referenceObject = null;
        }

        private bool HasCopiedOriginal()
        {
            var materials = serializedObject.FindProperty(PROPERTY_SERIALIZED_MATERIALS);
            var count = materials.arraySize;

            var originals = new List<Material>();
            for (var i = 0; i < count; i++)
            {
                var prop = materials.GetArrayElementAtIndex(i).FindPropertyRelative(PROPERTY_ORIGINAL);
                var mat = prop.objectReferenceValue;
                originals.Add(mat as Material);
            }
            originals = originals.Where(m => m != null).ToList();

            return originals.Count != originals.Distinct().Count();
        }
    }
}
