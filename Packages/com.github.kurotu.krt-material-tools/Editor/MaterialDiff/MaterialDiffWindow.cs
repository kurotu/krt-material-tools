using KRT.MaterialTools.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KRT.MaterialTools.MaterialDiff
{
    public class MaterialDiffWindow : EditorWindow
    {
        public Material leftMat;
        public Material rightMat;
        public Dictionary<ShaderUtil.ShaderPropertyType, bool> foldouts =
            new Dictionary<ShaderUtil.ShaderPropertyType, bool>(
                Enum.GetValues(typeof(ShaderUtil.ShaderPropertyType))
                    .Cast<ShaderUtil.ShaderPropertyType>()
                    .Select(t => KeyValuePair.Create(t, true)
                )
            );
        public bool foldoutKeywords = true;

        private Vector2 scrollPosition;

        private void OnEnable()
        {
            titleContent.text = "Material Diff";
            foldouts = new Dictionary<ShaderUtil.ShaderPropertyType, bool>();
            var propTypes = Enum.GetValues(typeof(ShaderUtil.ShaderPropertyType)).Cast<ShaderUtil.ShaderPropertyType>();
        }

        private void OnGUI()
        {
            if (UpdateChecker.ShouldNotifyUpdate())
            {
                UpdateChecker.NotifyUpdateGUI();
                EditorGUILayout.Space();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Materials");
                leftMat = (Material)EditorGUILayout.ObjectField(leftMat, typeof(Material), false);
                if (GUILayout.Button(new GUIContent("ÅÃ", "Swap left and right"), GUILayout.Width(80)))
                {
                    OnClickSwapMaterials();
                }
                rightMat = (Material)EditorGUILayout.ObjectField(rightMat, typeof(Material), false);
            }

            EditorGUILayout.Space();

            using var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition);
            scrollPosition = scrollView.scrollPosition;
            if (leftMat && rightMat)
            {
                // Shader
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Shader");
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.ObjectField(leftMat.shader, typeof(Shader), false);
                    }
                    using (new EditorGUI.DisabledScope(leftMat.shader == rightMat.shader))
                    {
                        if (GUILayout.Button(new GUIContent("Å®", "Copy to right"), GUILayout.Width(40)))
                        {
                            OnClickCopyShader(rightMat, leftMat.shader);
                        }
                        if (GUILayout.Button(new GUIContent("Å©", "Copy to left"), GUILayout.Width(40)))
                        {
                            OnClickCopyShader(leftMat, rightMat.shader);
                        }
                    }
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.ObjectField(rightMat.shader, typeof(Shader), false);
                    }
                }

                // Keywords
                foldoutKeywords = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutKeywords, "Keywords");
                if (foldoutKeywords)
                {
                    var leftKeywords = leftMat.shaderKeywords;
                    var rightKeywords = rightMat.shaderKeywords;
                    var allKeywords = leftKeywords.Concat(rightKeywords).Distinct().OrderBy(s => s).ToArray();
                    var hasKeywordDiff = false;
                    foreach (var keyword in allKeywords)
                    {
                        var leftHas = leftKeywords.Contains(keyword);
                        var rightHas = rightKeywords.Contains(keyword);
                        if (leftHas && rightHas)
                        {
                            continue;
                        }
                        hasKeywordDiff = true;
                        using var horizontal = new EditorGUILayout.HorizontalScope();
                        EditorGUI.indentLevel++;
                        EditorGUILayout.LabelField(keyword);
                        EditorGUI.indentLevel--;
                        EditorGUILayout.LabelField(leftHas ? "Yes" : "No");
                        if (GUILayout.Button(new GUIContent("Å®", "Copy to right"), GUILayout.Width(40)))
                        {
                            if (leftHas)
                            {
                                OnClickEnableKeyword(rightMat, keyword);
                            }
                            else
                            {
                                OnClickDisableKeyword(rightMat, keyword);
                            }
                        }
                        if (GUILayout.Button(new GUIContent("Å©", "Copy to left"), GUILayout.Width(40)))
                        {
                            if (rightHas)
                            {
                                OnClickEnableKeyword(leftMat, keyword);
                            }
                            else
                            {
                                OnClickDisableKeyword(leftMat, keyword);
                            }
                        }
                        EditorGUILayout.LabelField(rightHas ? "Yes" : "No");
                    }
                    if (!hasKeywordDiff)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.LabelField("No difference");
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();

                // Properties
                var propTypes = Enum.GetValues(typeof(ShaderUtil.ShaderPropertyType)).Cast<ShaderUtil.ShaderPropertyType>();
                foreach (var propType in propTypes)
                {
                    foldouts[propType] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts.ContainsKey(propType) && foldouts[propType], propType.ToString());
                    if (foldouts[propType])
                    {
                        bool hasDiff = false;
                        foreach (var propName in GetPropertyNames(propType, leftMat, rightMat))
                        {
                            using var horizontal = new EditorGUILayout.HorizontalScope();
                            var leftValue = new MaterialPropertyValue(leftMat, propType, propName);
                            var rightValue = new MaterialPropertyValue(rightMat, propType, propName);
                            if (leftValue.SameValue(rightValue))
                            {
                                continue;
                            }
                            hasDiff = true;
                            EditorGUI.indentLevel++;
                            EditorGUILayout.LabelField(propName);
                            EditorGUI.indentLevel--;
                            using (new EditorGUI.DisabledScope(true))
                            {
                                DrawPropertyValueField(leftValue);
                            }
                            using (new EditorGUI.DisabledScope(!leftValue.HasProperty))
                            {
                                if (GUILayout.Button(new GUIContent("Å®", "Copy to right"), GUILayout.Width(40)))
                                {
                                    OnClickCopyProperty(rightMat, propName, leftValue);
                                }
                            }
                            using (new EditorGUI.DisabledScope(!rightValue.HasProperty))
                            {
                                if (GUILayout.Button(new GUIContent("Å©", "Copy to left"), GUILayout.Width(40)))
                                {
                                    OnClickCopyProperty(leftMat, propName, rightValue);
                                }
                            }
                            using (new EditorGUI.DisabledScope(true))
                            {
                                DrawPropertyValueField(rightValue);
                            }
                        }
                        if (!hasDiff)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.LabelField("No difference");
                            EditorGUI.indentLevel--;
                        }
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
            }
        }

        private void OnClickSwapMaterials()
        {
            var tmp = leftMat;
            leftMat = rightMat;
            rightMat = tmp;
        }

        private void OnClickCopyShader(Material target, Shader shader)
        {
            Undo.RecordObject(target, $"Copy Shader to {target.name}");
            target.shader = shader;
        }

        private void OnClickEnableKeyword(Material target, string keyword)
        {
            Undo.RecordObject(target, $"Copy keyword {keyword} to {target.name}");
            target.EnableKeyword(keyword);
        }

        private void OnClickDisableKeyword(Material target, string keyword)
        {
            Undo.RecordObject(target, $"Disable keyword {keyword} of {target.name}");
            target.DisableKeyword(keyword);
        }

        private void OnClickCopyProperty(Material target, string propName, MaterialPropertyValue value)
        {
            Undo.RecordObject(target, $"Copy {propName} to {target.name}");
            SetPropertyValue(target, propName, value);
        }

        private void DrawPropertyValueField(MaterialPropertyValue value)
        {
            if (value.HasProperty)
            {
                switch (value.Type)
                {
                    case ShaderUtil.ShaderPropertyType.Color:
                        EditorGUILayout.ColorField(value.GetColor());
                        break;
                    case ShaderUtil.ShaderPropertyType.Vector:
                        EditorGUILayout.Vector4Field("", value.GetVector());
                        break;
                    case ShaderUtil.ShaderPropertyType.Float:
                        EditorGUILayout.FloatField(value.GetFloat());
                        break;
                    case ShaderUtil.ShaderPropertyType.Range:
                        EditorGUILayout.FloatField(value.GetRange());
                        break;
                    case ShaderUtil.ShaderPropertyType.TexEnv:
                        EditorGUILayout.ObjectField(value.GetTexture(), typeof(Texture), false);
                        break;
                    case ShaderUtil.ShaderPropertyType.Int:
                        EditorGUILayout.IntField(value.GetInt());
                        break;
                }
            }
            else
            {
                var style = new GUIStyle(EditorStyles.label);
                style.fixedWidth = 0;
                EditorGUILayout.LabelField("None", GUILayout.MinWidth(50));
            }
        }

        private string[] GetPropertyNames(ShaderUtil.ShaderPropertyType materialPropertyType, Material left, Material right)
        {
            var leftPropNames = GetPropertyNames(left.shader, materialPropertyType);
            var rightPropNames = GetPropertyNames(right.shader, materialPropertyType);
            return leftPropNames.Concat(rightPropNames).Distinct().OrderBy(s => s).ToArray();
        }

        private string[] GetPropertyNames(Shader s, ShaderUtil.ShaderPropertyType type)
        {
            var propCount = ShaderUtil.GetPropertyCount(s);
            var propNames = new string[propCount];
            for (int i = 0; i < propCount; i++)
            {
                propNames[i] = ShaderUtil.GetPropertyName(s, i);
            }
            return propNames.Where((n, i) => ShaderUtil.GetPropertyType(s, i) == type).ToArray();
        }

        private void SetPropertyValue(Material mat, string name, MaterialPropertyValue value)
        {
            switch (value.Type)
            {
                case ShaderUtil.ShaderPropertyType.Color:
                    mat.SetColor(name, value.GetColor());
                    break;
                case ShaderUtil.ShaderPropertyType.Vector:
                    mat.SetVector(name, value.GetVector());
                    break;
                case ShaderUtil.ShaderPropertyType.Float:
                    mat.SetFloat(name, value.GetFloat());
                    break;
                case ShaderUtil.ShaderPropertyType.Range:
                    mat.SetFloat(name, value.GetRange());
                    break;
                case ShaderUtil.ShaderPropertyType.TexEnv:
                    mat.SetTexture(name, value.GetTexture());
                    break;
                case ShaderUtil.ShaderPropertyType.Int:
                    mat.SetInt(name, value.GetInt());
                    break;
            }
        }
    }
}
