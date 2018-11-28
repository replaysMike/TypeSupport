using System;

namespace TypeSupport.Extensions
{
    /// <summary>
    /// Specify field retrieval options
    /// </summary>
    [Flags]
    public enum FieldOptions : byte
    {
        /// <summary>
        /// All fields
        /// </summary>
        All = 0,
        /// <summary>
        /// All publicly defined fields
        /// </summary>
        Public = 1,
        /// <summary>
        /// All privately defined fields
        /// </summary>
        Private = 2,
        /// <summary>
        /// All static fields
        /// </summary>
        Static = 4,
        /// <summary>
        /// All auto-property backing fields
        /// </summary>
        BackingFields = 8,
        /// <summary>
        /// All constant fields
        /// </summary>
        Constants = 16,
        /// <summary>
        /// All writable fields (all fields except constants)
        /// </summary>
        AllWritable = 32
    }
}
