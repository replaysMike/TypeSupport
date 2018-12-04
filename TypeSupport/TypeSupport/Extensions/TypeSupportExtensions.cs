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
            return GetExtendedType(type, TypeSupportOptions.All);
        }

        /// <summary>
        /// Get the extended type for a Type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="options">The type support inspection options</param>
        /// <returns></returns>
        public static ExtendedType GetExtendedType(this Type type, TypeSupportOptions options)
        {
            return new ExtendedType(type, options);
        }

        /// <summary>
        /// Get the extended type for a Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ExtendedType GetExtendedType(this object type)
        {
            return GetExtendedType(type, TypeSupportOptions.All);
        }

        /// <summary>
        /// Get the extended type for a Type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="options">The type support inspection options</param>
        /// <returns></returns>
        public static ExtendedType GetExtendedType(this object type, TypeSupportOptions options)
        {
            return new ExtendedType(type.GetType(), options);
        }
    }
}
