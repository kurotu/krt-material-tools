using KRT.MaterialTools.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KRT.MaterialTools.MaterialReplacer
{
    /// <summary>
    /// Represents material override setting object.
    /// </summary>
    [CreateAssetMenu(fileName = "MaterialReplacerRule", menuName = MenuEntry.CreateAssetMenu.MaterialReplacerRule, order = (int)MenuEntry.CreateAssetMenu.Priority.MaterialReplacerRule)]
    public class MaterialReplacerRule : ScriptableObject, ISerializationCallbackReceiver
    {
        private Dictionary<Material, Material> m_Materials = new Dictionary<Material, Material>();

        [SerializeField]
        private SerializablePair[] m_SerializedMaterials;

        /// <summary>
        /// Returns the replaced material.
        /// </summary>
        /// <param name="key">Key to search the original material.</param>
        /// <returns>Overriding material.</returns>
        public Material this[Material key]
        {
            get
            {
                return m_Materials[key];
            }

            set
            {
                m_Materials[key] = value;
            }
        }

        /// <summary>
        /// Gets the list of Material pairs.
        /// </summary>
        /// <param name="pairs">Array to receive results.</param>
        public void GetPairs(List<KeyValuePair<Material, Material>> pairs)
        {
            pairs.Clear();
            pairs.AddRange(m_Materials);
        }

        /// <summary>
        /// Gets whether the key is contained as an original material.
        /// </summary>
        /// <param name="key">Key to search the original material.</param>
        /// <returns>true when the key is contained.</returns>
        public bool ContainsKey(Material key)
        {
            return m_Materials.ContainsKey(key);
        }

        /// <summary>
        /// Called before Unity serialization.
        /// </summary>
        public void OnBeforeSerialize()
        {
            m_SerializedMaterials = m_Materials.Select(pair => new SerializablePair() { original = pair.Key, replaced = pair.Value }).ToArray();
        }

        /// <summary>
        /// Called after Unity deserialization.
        /// </summary>
        public void OnAfterDeserialize()
        {
            m_Materials = m_SerializedMaterials.ToDictionary(pair => pair.original, pair => pair.replaced);
        }

        [System.Serializable]
        private class SerializablePair
        {
            public Material original;
            public Material replaced;
        }
    }
}
