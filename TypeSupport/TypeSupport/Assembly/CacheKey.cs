using System;

namespace TypeSupport.Assembly
{
    /// <summary>
    /// A composite cache key
    /// </summary>
    public struct CacheKey
    {
        public Type Type;
        public TypeSupportOptions Options;
        public CacheKey(Type type, TypeSupportOptions options)
        {
            Type = type;
            Options = options;
        }

        public override int GetHashCode()
        {
            var hashCode = 23;
            hashCode = hashCode * 31 + (int)Options;
            hashCode = hashCode * 31 + Type.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(CacheKey))
                return false;
            var objTyped = (CacheKey)obj;
            return objTyped.Options == Options && objTyped.Type.Equals(Type);
        }

        public override string ToString() => $"{(int)Options}-({Options}) {Type.FullName}";
    }
}
