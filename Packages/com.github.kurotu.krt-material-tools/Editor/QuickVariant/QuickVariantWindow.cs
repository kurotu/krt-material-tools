﻿using KRT.MaterialTools.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KRT.MaterialTools.QuickVariant
{
    internal class QuickVariantWindow : CommonEditorWindowBase
    {
        private const string DefaultPrefix = "";
        private const string DefaultVariantSuffix = " Variant";
        private const string DefaultCopySuffix = " Copy";

        public GameObject Target;
        public List<Material> SelectedMaterials = new List<Material>();
        public bool CreateAsVariant = true;
        public string Prefix = DefaultPrefix;
        public string Suffix = DefaultVariantSuffix;
        public bool ApplyToRenderers = true;
        private Vector2 _scrollPosition;

        private void OnEnable()
        {
            titleContent = new GUIContent("Quick Variant");
        }

        protected override void OnGUIInternal()
        {
            var selectedMaterial = (GameObject)EditorGUILayout.ObjectField("Target", Target, typeof(GameObject), true);
            if (selectedMaterial != Target)
            {
                Target = selectedMaterial;
                if (Target != null)
                {
                    OnChangeTargetGameObject();
                }
            }

            if (Target == null)
            {
                EditorGUILayout.HelpBox("Please select a GameObject", MessageType.Info);
                return;
            }

            using var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition);
            _scrollPosition = scrollView.scrollPosition;

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Select All"))
                {
                    OnClickSelectAll();
                }
                if (GUILayout.Button("Clear"))
                {
                    OnClickClear();
                }
            }

            foreach (var material in GetMaterials(Target))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    var current = SelectedMaterials.Contains(material);
                    var selected = EditorGUILayout.Toggle(current, GUILayout.Width(20));
                    var changed = selected != current;
                    if (changed)
                    {
                        if (selected)
                        {
                            SelectedMaterials.Add(material);
                        }
                        else
                        {
                            SelectedMaterials.Remove(material);
                        }
                    }
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.ObjectField(material, typeof(Material), false);
                    }
                }
            }

            EditorGUILayout.Space();

#if UNITY_2022_1_OR_NEWER
            CreateAsVariant = EditorGUILayout.Toggle("Use Material Variant", CreateAsVariant);
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
            ApplyToRenderers = EditorGUILayout.Toggle("Apply to Renderers", ApplyToRenderers);

            EditorGUILayout.Space();

            Prefix = EditorGUILayout.TextField("Prefix", Prefix);
            Suffix = EditorGUILayout.TextField("Suffix", Suffix);
            var sampleMaterialName = SelectedMaterials.FirstOrDefault()?.name ?? "<Material Name>";
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Example");
                EditorGUILayout.LabelField($"{Prefix}{sampleMaterialName}{Suffix}");
            }

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(SelectedMaterials.Count == 0))
            {
                if (GUILayout.Button("Create Materials"))
                {
                    OnClickCreateMaterials();
                }
            }

            EditorGUILayout.Space();
        }

        public void SetTarget(GameObject go)
        {
            Target = go;
            OnChangeTargetGameObject();
        }

        #region Event Handlers

        private void OnClickSelectAll()
        {
            SelectAllMaterials();
        }

        private void OnClickClear()
        {
            SelectedMaterials.Clear();
        }

        private void OnChangeTargetGameObject()
        {
            SelectAllMaterials();
        }

        private void OnClickCreateMaterials()
        {
            var directory = CommonUtil.SelectAssetDirectory("Save Materials");
            if (string.IsNullOrEmpty(directory))
            {
                return;
            }

            var materials = SelectedMaterials.ToArray();
#if UNITY_2022_1_OR_NEWER
            var variant = CreateAsVariant;
#else
            var variant = false;
#endif

            var duplicatedMaterials = CommonUtil.DuplicateMaterials(materials, directory, Prefix, Suffix, variant);
            if (ApplyToRenderers)
            {
                foreach (var renderer in Target.GetComponentsInChildren<Renderer>(true))
                {
                    Undo.RecordObject(renderer, "Apply duplicated materials");
                    renderer.sharedMaterials = renderer.sharedMaterials
                        .Select(m =>
                        {
                            if (m == null)
                            {
                                return null;
                            }
                            return duplicatedMaterials.TryGetValue(m, out var newMaterial) ? newMaterial : m;
                        })
                        .ToArray();
                }
            }
            SelectedMaterials.Clear();
        }

        #endregion

        private void SelectAllMaterials()
        {
            SelectedMaterials = new List<Material>(GetMaterials(Target));
        }

        private static Material[] GetMaterials(GameObject go)
        {
            var rendererMaterials = go.GetComponentsInChildren<Renderer>(true)
                .SelectMany(r => r.sharedMaterials);
            var animatorMaterials = go.GetComponentsInChildren<Animator>(true)
                .Where(a => a.runtimeAnimatorController != null)
                .SelectMany(a => a.runtimeAnimatorController.animationClips)
                .SelectMany(GetMaterials);
#if KMT_VRCSDK3A
            var av3Materials = go.GetComponentsInChildren<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>(true)
                .SelectMany(d => d.baseAnimationLayers)
                .Where(l => !l.isDefault && l.animatorController != null)
                .SelectMany(l => l.animatorController.animationClips)
                .SelectMany(GetMaterials);
#endif

#if KMT_MODULAR_AVATAR
            var maMaterials = go.GetComponentsInChildren<nadena.dev.modular_avatar.core.ModularAvatarMergeAnimator>(true)
                .SelectMany(ma => ma.animator.animationClips)
                .SelectMany(GetMaterials);
#endif

            return rendererMaterials
                .Concat(animatorMaterials)
#if KMT_VRCSDK3A
                .Concat(av3Materials)
#endif
#if KMT_MODULAR_AVATAR
                .Concat(maMaterials)
#endif
                .Where(m => m != null)
                .Distinct().OrderBy(m => m.name).ToArray();
        }

        private static Material[] GetMaterials(AnimationClip clip)
        {
            var bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
            var keyframes = bindings.SelectMany(b => AnimationUtility.GetObjectReferenceCurve(clip, b));
            var materials = keyframes.Where(k => k.value is Material)
                .Select(k => (Material)k.value);
            return materials.ToArray();
        }
    }
}
