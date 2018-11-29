using System;

namespace TypeSupport.Extensions
{
    [Flags]
    public enum ConstructorOptions : byte
    {
        /// <summary>
        /// Get all constructors
        /// </summary>
        All = 0,
        /// <summary>
        /// Get only empty constructors
        /// </summary>
        EmptyConstructor = 1,
        /// <summary>
        /// Get only non-empty constructors
        /// </summary>
        NonEmptyConstructor = 2,
    }
}
