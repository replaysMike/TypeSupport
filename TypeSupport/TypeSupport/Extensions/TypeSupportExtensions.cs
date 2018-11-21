using System;

namespace TypeSupport.Extensions
{
    /// <summary>
    /// Extensions for TypeSupport
    /// </summary>
    public static class TypeSupportExtensions
    {
        /// <summary>
        /// Get the Type Support for a Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TypeSupport TypeSupport(this Type type)
        {
            return new TypeSupport(type);
        }
    }
}
