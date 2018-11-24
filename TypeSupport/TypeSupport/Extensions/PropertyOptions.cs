using System;

namespace TypeSupport.Extensions
{
    /// <summary>
    /// Specify property retrieval options
    /// </summary>
    [Flags]
    public enum PropertyOptions : byte
    {
        /// <summary>
        /// All properties
        /// </summary>
        All = 0,
        /// <summary>
        /// All properties with public accessors
        /// </summary>
        Public = 1,
        /// <summary>
        /// All properties with private accessors
        /// </summary>
        Private = 2,
        /// <summary>
        /// Properties with a setter defined
        /// </summary>
        HasSetter = 4,
        /// <summary>
        /// Properties with a getter defined
        /// </summary>
        HasGetter = 8,
        /// <summary>
        /// Properties that contain a collection indexer
        /// </summary>
        HasIndexer = 16,
    }
}
