﻿using System;
using UnityEditor;
using UnityEngine;

namespace KRT.MaterialTools.MaterialDiff
{
    internal class MaterialPropertyValue
    {
        public ShaderUtil.ShaderPropertyType Type { get; }
        public bool HasProperty { get; }

        private object Value { get; }

        public MaterialPropertyValue(ShaderUtil.ShaderPropertyType type, object value)
        {
            Type = type;
            Value = value;
            HasProperty = true;
        }

        public MaterialPropertyValue(Material material, ShaderUtil.ShaderPropertyType type, string propName)
        {
            Type = type;
            switch (type)
            {
                case ShaderUtil.ShaderPropertyType.Color:
                    HasProperty = material.HasColor(propName);
                    Value = HasProperty ? material.GetColor(propName) : null;
                    break;
                case ShaderUtil.ShaderPropertyType.Vector:
                    HasProperty = material.HasVector(propName);
                    Value = HasProperty ? material.GetVector(propName) : null;
                    break;
                case ShaderUtil.ShaderPropertyType.Float:
                    HasProperty = material.HasFloat(propName);
                    Value = HasProperty ? material.GetFloat(propName) : null;
                    break;
                case ShaderUtil.ShaderPropertyType.Range:
                    HasProperty = material.HasFloat(propName);
                    Value = HasProperty ? material.GetFloat(propName) : null;
                    break;
                case ShaderUtil.ShaderPropertyType.TexEnv:
                    HasProperty = material.HasTexture(propName);
                    Value = HasProperty ? material.GetTexture(propName) : null;
                    break;
                case ShaderUtil.ShaderPropertyType.Int:
                    HasProperty = material.HasInt(propName);
                    Value = HasProperty ? material.GetInt(propName) : null;
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
            if (Value == null)
            {
                return other.Value == null;
            }
            if (other.Value == null)
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
