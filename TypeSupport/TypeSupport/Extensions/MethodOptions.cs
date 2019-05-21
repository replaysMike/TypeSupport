using System;

namespace TypeSupport.Extensions
{
    /// <summary>
    /// Specify method retrieval options
    /// </summary>
    [Flags]
    public enum MethodOptions
    {
        /// <summary>
        /// All methods
        /// </summary>
        All = 0,
        /// <summary>
        /// All methods declared public
        /// </summary>
        Public = 1,
        /// <summary>
        /// All methods declared private
        /// </summary>
        Private = 2,
        /// <summary>
        /// All methods declared static
        /// </summary>
        Static = 4,
        /// <summary>
        /// All methods declared as a constructor
        /// </summary>
        Constructor = 8,
        /// <summary>
        /// All methods which are base implementation
        /// </summary>
        BaseImplementation = 16,
        /// <summary>
        /// All methods which are overriding base implementation
        /// </summary>
        Overridden = 32,
        /// <summary>
        /// All methods declared virtual
        /// </summary>
        Virtual = 64,
        /// <summary>
        /// All methods acting as property accessors
        /// </summary>
        AutoPropertyAccessor = 128,
    }
}
