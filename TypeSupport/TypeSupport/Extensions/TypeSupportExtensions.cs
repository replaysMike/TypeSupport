using System;

namespace TypeSupport.Extensions
{
    /// <summary>
    /// Extensions for Type Support
    /// </summary>
    public static class TypeSupportExtensions
    {
        /// <summary>
        /// Get the extended type for a Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ExtendedType GetExtendedType(this Type type)
        {
            return new ExtendedType(type);
        }
    }
}
