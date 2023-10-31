using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KRT.MaterialTools.TextureReplacer
{
    [Serializable]
    internal class TextureReplacerRule
    {
        [SerializeField]
        private List<TexturePair> texturePairs = new List<TexturePair>();

        internal bool IsBlank => texturePairs.Where(pair => pair.replacement != null).Count() == 0;

        internal void SetTexture(Texture key, Texture value)
        {
            var index = texturePairs.FindIndex(p => p.original == key);
            if (index >= 0)
            {
                texturePairs[index].replacement = value;
            }
            else
            {
                texturePairs.Add(new TexturePair { original = key, replacement = value });
            }
        }

        internal Texture GetTexture(Texture key)
        {
            return texturePairs.FirstOrDefault(p => p.original == key)?.replacement;
        }

        internal bool ContainsKey(Texture key)
        {
            return texturePairs.Any(p => p.original == key);
        }

        internal Dictionary<Texture, Texture> ToDictionary()
        {
            return texturePairs.ToDictionary(p => p.original, p => p.replacement);
        }

        internal void Clear()
        {
            texturePairs.Clear();
        }

        [Serializable]
        private class TexturePair
        {
            public Texture original;
            public Texture replacement;
        }
    }
}
