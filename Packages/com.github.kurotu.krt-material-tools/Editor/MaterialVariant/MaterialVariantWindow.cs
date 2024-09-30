using KRT.MaterialTools.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KRT.MaterialTools.MaterialVariant
{
    internal class MaterialVariantWindow : CommonEditorWindowBase
    {
        private const string DefaultPrefix = "";
        private const string DefaultVariantSuffix = " Variant";
        private const string DefaultCopySuffix = " Copy";

        public List<Material> Materials = new List<Material>();
        public bool CreateAsVariant = true;
        public string Prefix = DefaultPrefix;
        public string Suffix = DefaultVariantSuffix;
        private Vector2 _scrollPosition;

        private void OnEnable()
        {
            titleContent = new GUIContent("Material Variant");
        }

        protected override void OnGUIInternal()
        {
            using var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition);
            _scrollPosition = scrollView.scrollPosition;

            var so = new SerializedObject(this);
            so.Update();
            var sMaterials = so.FindProperty("Materials");
            EditorGUILayout.PropertyField(sMaterials, true);
            so.ApplyModifiedProperties();

            EditorGUILayout.Space();

#if UNITY_2022_1_OR_NEWER
            CreateAsVariant = EditorGUILayout.Toggle("Create Material Variant", CreateAsVariant);
#else
            CreateAsVariant = false;
#endif
            if (CreateAsVariant && Suffix == DefaultCopySuffix)
            {
                Suffix = DefaultVariantSuffix;
            }
            else if (!CreateAsVariant && Suffix == DefaultVariantSuffix)
            {
                Suffix = DefaultCopySuffix;
            }

            EditorGUILayout.Space();

            Prefix = EditorGUILayout.TextField("Prefix", Prefix);
            Suffix = EditorGUILayout.TextField("Suffix", Suffix);
            var firstNotNull = Materials.Where(m => m != null).FirstOrDefault();
            var sampleMaterialName = firstNotNull?.name ?? "<Material Name>";
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Example");
                EditorGUILayout.LabelField($"{Prefix}{sampleMaterialName}{Suffix}");
            }

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(firstNotNull == null))
            {
                if (GUILayout.Button("Create Materials"))
                {
                    OnClickCreateMaterials();
                }
            }

            EditorGUILayout.Space();
        }

        private void OnClickCreateMaterials()
        {
            var directory = CommonUtil.SelectAssetDirectory("Save Materials");
            if (string.IsNullOrEmpty(directory))
            {
                return;
            }

            var materials = Materials.Where(m => m != null).ToArray();
#if UNITY_2022_1_OR_NEWER
            var variant = CreateAsVariant;
#else
            var variant = false;
#endif

            var duplicatedMaterials = CommonUtil.DuplicateMaterials(materials, directory, Prefix, Suffix, variant);
            Materials.Clear();
        }
    }
}
