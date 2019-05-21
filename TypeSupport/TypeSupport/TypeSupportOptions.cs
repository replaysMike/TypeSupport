using System;

namespace TypeSupport
{
    /// <summary>
    /// Type support options
    /// </summary>
    [Flags]
    public enum TypeSupportOptions : int
    {
        /// <summary>
        /// Load and inspect custom attributes for object
        /// </summary>
        Attributes = 1,
        /// <summary>
        /// Load and inspect collection information
        /// </summary>
        Collections = 2,
        /// <summary>
        /// Load and discover all concrete types for an interface
        /// </summary>
        ConcreteTypes = 4,
        /// <summary>
        /// Load all constructor information
        /// </summary>
        Constructors = 8,
        /// <summary>
        /// Load and inspect enum information for an object
        /// </summary>
        Enums = 16,
        /// <summary>
        /// Load and inspect fields for object
        /// </summary>
        Fields = 32,
        /// <summary>
        /// Load and inspect generics information
        /// </summary>
        Generics = 64,
        /// <summary>
        /// Load and inspect indexers object
        /// </summary>
        Indexers = 128,
        /// <summary>
        /// Load and inspect properties for object
        /// </summary>
        Properties = 256,
		/// <summary>
		/// Enable type caching
		/// </summary>
		Caching = 512,
        /// <summary>
        /// Load and inspect methods
        /// </summary>
        Methods = 1024,
        /// <summary>
        /// Uses all default options
        /// </summary>
        All = Attributes | Collections | ConcreteTypes | Constructors | Enums | Fields | Generics | Indexers | Properties | Caching | Methods,
    }
}
