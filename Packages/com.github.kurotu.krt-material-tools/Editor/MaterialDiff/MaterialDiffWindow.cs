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
        public Dictionary<ShaderUtil.ShaderPropertyType, bool> foldouts;

        private Vector2 scrollPosition;

        private void OnEnable()
        {
            titleContent.text = "Material Diff";
            foldouts = new Dictionary<ShaderUtil.ShaderPropertyType, bool>();
            var propTypes = Enum.GetValues(typeof(ShaderUtil.ShaderPropertyType)).Cast<ShaderUtil.ShaderPropertyType>();
            foreach (var propType in propTypes)
            {
                foldouts[propType] = true;
            }
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
                if (GUILayout.Button(new GUIContent("ÅÃ", "Swap left and right")))
                {
                    OnClickSwap();
                }
                rightMat = (Material)EditorGUILayout.ObjectField(rightMat, typeof(Material), false);
            }

            using var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition);
            scrollPosition = scrollView.scrollPosition;
            if (leftMat && rightMat)
            {
                var propTypes = Enum.GetValues(typeof(ShaderUtil.ShaderPropertyType)).Cast<ShaderUtil.ShaderPropertyType>();
                foreach (var propType in propTypes)
                {
                    foldouts[propType] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts.ContainsKey(propType) && foldouts[propType], propType.ToString());
                    if (foldouts[propType])
                    {
                        bool hasDiff = false;
                        EditorGUI.indentLevel++;
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
                            EditorGUILayout.LabelField(propName);
                            using (new EditorGUI.DisabledScope(true))
                            {
                                DrawPropertyValueField(leftValue);
                            }
                            if (GUILayout.Button(new GUIContent("Å®", "Copy to right")))
                            {
                                OnClickCopyToRight(propName, leftValue);
                            }
                            if (GUILayout.Button(new GUIContent("Å©", "Copy to left")))
                            {
                                OnClickCopyToLeft(propName, rightValue);
                            }
                            using (new EditorGUI.DisabledScope(true))
                            {
                                DrawPropertyValueField(rightValue);
                            }
                        }
                        if (!hasDiff)
                        {
                            EditorGUILayout.LabelField("No difference");
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
            }
        }

        private void OnClickCopyToLeft(string propName, MaterialPropertyValue rightValue)
        {
            SetPropertyValue(leftMat, propName, rightValue);
        }

        private void OnClickCopyToRight(string propName, MaterialPropertyValue leftValue)
        {
            SetPropertyValue(rightMat, propName, leftValue);
        }

        private void OnClickSwap()
        {
            var tmp = leftMat;
            leftMat = rightMat;
            rightMat = tmp;
        }

        private void DrawPropertyValueField(MaterialPropertyValue value)
        {
            switch (value.Type)
            {
                case ShaderUtil.ShaderPropertyType.Color:
                    if (value.Value == null)
                    {
                        EditorGUILayout.LabelField("None");
                    }
                    EditorGUILayout.ColorField(value.GetColor());
                    break;
                case ShaderUtil.ShaderPropertyType.Vector:
                    if (value.Value == null)
                    {
                        EditorGUILayout.LabelField("None");
                    }
                    EditorGUILayout.Vector4Field("", value.GetVector());
                    break;
                case ShaderUtil.ShaderPropertyType.Float:
                    if (value.Value == null)
                    {
                        EditorGUILayout.LabelField("None");
                    }
                    else
                    {
                        EditorGUILayout.FloatField(value.GetFloat());
                    }
                    break;
                case ShaderUtil.ShaderPropertyType.Range:
                    if (value.Value == null)
                    {
                        EditorGUILayout.LabelField("None");
                    }
                    else
                    {
                        EditorGUILayout.FloatField(value.GetRange());
                    }
                    break;
                case ShaderUtil.ShaderPropertyType.TexEnv:
                    EditorGUILayout.ObjectField(value.GetTexture(), typeof(Texture), false);
                    break;
                case ShaderUtil.ShaderPropertyType.Int:
                    if (value.Value == null)
                    {
                        EditorGUILayout.LabelField("None");
                    }
                    else
                    {
                        EditorGUILayout.IntField(value.GetInt());
                    }
                    break;
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
