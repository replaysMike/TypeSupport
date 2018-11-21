using System;

namespace TypeSupport
{
    /// <summary>
    /// A type support exception
    /// </summary>
    public class TypeSupportException : Exception
    {
        Type RequestedType { get; }

        /// <summary>
        /// A type support exception
        /// </summary>
        /// <param name="message"></param>
        public TypeSupportException(string message) : base(message)
        {

        }

        /// <summary>
        /// A type support exception
        /// </summary>
        /// <param name="requestedType"></param>
        /// <param name="message"></param>
        public TypeSupportException(Type requestedType, string message) : base(message)
        {
            RequestedType = requestedType;
        }
    }
}
