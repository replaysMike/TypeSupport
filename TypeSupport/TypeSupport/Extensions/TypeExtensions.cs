using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TypeSupport.Extensions
{
    /// <summary>
    /// Type extensions
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Get all of the properties of an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ICollection<ExtendedProperty> GetProperties(this Type type, PropertyOptions options)
        {
            var allProperties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            IEnumerable<PropertyInfo> returnProperties = allProperties;

            if (options.HasFlag(PropertyOptions.Public))
                returnProperties = returnProperties.Where(x => x.GetGetMethod(true).IsPublic && x.GetSetMethod(true).IsPublic);
            if (options.HasFlag(PropertyOptions.Private))
                returnProperties = returnProperties.Where(x => x.GetGetMethod(true).IsPrivate && x.GetSetMethod(true).IsPrivate);
            if (options.HasFlag(PropertyOptions.HasSetter))
                returnProperties = returnProperties.Where(x => x.SetMethod != null);
            if (options.HasFlag(PropertyOptions.HasGetter))
                returnProperties = returnProperties.Where(x => x.GetMethod != null);
            if (options.HasFlag(PropertyOptions.HasIndexer))
                returnProperties = returnProperties.Where(x => x.GetIndexParameters().Any());

            return returnProperties
                .Select(x => (ExtendedProperty)x).ToList();
        }

        /// <summary>
        /// Get all of the fields of an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="includeAutoPropertyBackingFields">True to include the compiler generated backing fields for auto-property getters/setters</param>
        /// <returns></returns>
        public static ICollection<ExtendedField> GetFields(this Type type, FieldOptions options)
        {
            var allFields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            IEnumerable<FieldInfo> returnFields = allFields;

            if (options.HasFlag(FieldOptions.Public))
                returnFields = returnFields.Where(x => x.IsPublic);
            if (options.HasFlag(FieldOptions.Private))
                returnFields = returnFields.Where(x => x.IsPrivate);
            if (options.HasFlag(FieldOptions.Static))
                returnFields = returnFields.Where(x => x.IsStatic);
            if (options.HasFlag(FieldOptions.BackingFields))
                returnFields = allFields.Where(x => x.Name.Contains("k__BackingField"));
            return returnFields.Select(x => (ExtendedField)x).ToList();
        }
    }
}
