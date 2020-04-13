using System;
using System.Collections.Generic;

namespace TypeSupport
{
    public interface IAttributeInspection
    {
        /// <summary>
        /// Returns true if object is decorated by attribute
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        bool HasAttribute<TAttribute>() where TAttribute : class;

        /// <summary>
        /// Returns true if object is decorated by attribute
        /// </summary>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        bool HasAttribute(Type attributeType);

        /// <summary>
        /// Returns the requested attribute if declared on object, null otherwise
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        TAttribute GetAttribute<TAttribute>() where TAttribute : class;

        /// <summary>
        /// Returns the requested attribute if declared on object, null otherwise
        /// </summary>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        Attribute GetAttribute(Type attributeType);

        /// <summary>
        /// Get all custom attributes on object
        /// </summary>
        /// <returns></returns>
        IEnumerable<Attribute> GetAttributes();

        /// <summary>
        /// Get all custom attributes on object
        /// </summary>
        /// <returns></returns>
        IEnumerable<TAttribute> GetAttributes<TAttribute>() where TAttribute : Attribute;
    }
}
