using System;
using UnityEditor;
using UnityEngine;

namespace KRT.MaterialTools.MaterialDiff
{
    internal class MaterialPropertyValue
    {
        public ShaderUtil.ShaderPropertyType Type { get; }
        public object Value { get; }

        public MaterialPropertyValue(ShaderUtil.ShaderPropertyType type, object value)
        {
            Type = type;
            Value = value;
        }

        public MaterialPropertyValue(Material material, ShaderUtil.ShaderPropertyType type, string propName)
        {
            Type = type;
            switch (type)
            {
                case ShaderUtil.ShaderPropertyType.Color:
                    Value = material.HasColor(propName) ? material.GetColor(propName) : null;
                    break;
                case ShaderUtil.ShaderPropertyType.Vector:
                    Value = material.HasVector(propName) ? material.GetVector(propName) : null;
                    break;
                case ShaderUtil.ShaderPropertyType.Float:
                    Value = material.HasFloat(propName) ? material.GetFloat(propName) : null;
                    break;
                case ShaderUtil.ShaderPropertyType.Range:
                    Value = material.HasFloat(propName) ? material.GetFloat(propName) : null;
                    break;
                case ShaderUtil.ShaderPropertyType.TexEnv:
                    Value = material.HasTexture(propName) ? material.GetTexture(propName) : null;
                    break;
                case ShaderUtil.ShaderPropertyType.Int:
                    Value = material.HasInt(propName) ? material.GetInt(propName) : null;
                    break;
            }
        }

        public Color GetColor()
        {
            if (Type != ShaderUtil.ShaderPropertyType.Color)
            {
                throw new InvalidOperationException("Type is not Color");
            }
            return (Color)Value;
        }

        public Vector4 GetVector()
        {
            if (Type != ShaderUtil.ShaderPropertyType.Vector)
            {
                throw new InvalidOperationException("Type is not Vector");
            }
            return (Vector4)Value;
        }

        public float GetRange()
        {
            if (Type != ShaderUtil.ShaderPropertyType.Range)
            {
                throw new InvalidOperationException("Type is not Range");
            }
            return (float)Value;
        }

        public float GetFloat()
        {
            if (Type != ShaderUtil.ShaderPropertyType.Float)
            {
                throw new InvalidOperationException("Type is not Float");
            }
            return (float)Value;
        }

        public Texture GetTexture()
        {
            if (Type != ShaderUtil.ShaderPropertyType.TexEnv)
            {
                throw new InvalidOperationException("Type is not Texture");
            }
            return (Texture)Value;
        }

        public int GetInt()
        {
            if (Type != ShaderUtil.ShaderPropertyType.Int)
            {
                throw new InvalidOperationException("Type is not Float");
            }
            return (int)Value;
        }

        public bool SameValue(MaterialPropertyValue other)
        {
            if (Type != other.Type)
            {
                return false;
            }
            switch (Type)
            {
                case ShaderUtil.ShaderPropertyType.Color:
                    return GetColor() == other.GetColor();
                case ShaderUtil.ShaderPropertyType.Vector:
                    return GetVector() == other.GetVector();
                case ShaderUtil.ShaderPropertyType.Float:
                    return GetFloat() == other.GetFloat();
                case ShaderUtil.ShaderPropertyType.Range:
                    return GetRange() == other.GetRange();
                case ShaderUtil.ShaderPropertyType.TexEnv:
                    return GetTexture() == other.GetTexture();
                case ShaderUtil.ShaderPropertyType.Int:
                    return GetInt() == other.GetInt();
                default:
                    return false;
            }
        }
    }
}
