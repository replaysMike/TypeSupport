using System;
using System.Collections.Generic;
using System.Text;

namespace TypeSupport
{
    /// <summary>
    /// Type support options
    /// </summary>
    [Flags]
    public enum TypeSupportOptions
    {
        /// <summary>
        /// Load and inspect custom attributes for object
        /// </summary>
        Attributes,
        /// <summary>
        /// Load and inspect collection information
        /// </summary>
        Collections,
        /// <summary>
        /// Load and discover all concrete types for an interface
        /// </summary>
        ConcreteTypes,
        /// <summary>
        /// Load all constructor information
        /// </summary>
        Constructors,
        /// <summary>
        /// Load and inspect enum information for an object
        /// </summary>
        Enums,
        /// <summary>
        /// Load and inspect fields for object
        /// </summary>
        Fields,
        /// <summary>
        /// Load and inspect generics information
        /// </summary>
        Generics,
        /// <summary>
        /// Load and inspect indexers object
        /// </summary>
        Indexers,
        /// <summary>
        /// Load and inspect properties for object
        /// </summary>
        Properties,
        /// <summary>
        /// Uses all default options
        /// </summary>
        All = Attributes | Collections | ConcreteTypes | Constructors | Enums | Fields | Generics | Indexers | Properties,
    }
}
