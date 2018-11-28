using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            if (type == null)
                return new List<ExtendedProperty>();
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var allProperties = type.GetProperties(flags);
            var allFields = type.GetProperties(flags)
                .Select(x => (ExtendedProperty)x)
                .Concat(GetProperties(type.BaseType, options));
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
            if (type == null)
                return new List<ExtendedField>();
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            var allFields = type.GetFields(flags)
                .Select(x => (ExtendedField)x)
                .Concat(GetFields(type.BaseType, options));
            IEnumerable<ExtendedField> returnFields = allFields;

            if (options.HasFlag(FieldOptions.AllWritable))
            {
                returnFields = returnFields.Where(x => !x.FieldInfo.IsLiteral);
            }

            if (options.HasFlag(FieldOptions.Public))
                returnFields = returnFields.Where(x => x.FieldInfo.IsPublic);
            if (options.HasFlag(FieldOptions.Private))
                returnFields = returnFields.Where(x => x.FieldInfo.IsPrivate);
            if (options.HasFlag(FieldOptions.Static))
                returnFields = returnFields.Where(x => x.FieldInfo.IsStatic);
            if (options.HasFlag(FieldOptions.BackingFields))
                returnFields = allFields.Where(x => x.Name.Contains("k__BackingField"));
            if (options.HasFlag(FieldOptions.Constants))
                returnFields = returnFields.Where(x => x.FieldInfo.IsLiteral);
            return returnFields.Select(x => (ExtendedField)x).ToList();
        }

        /// <summary>
        /// Get an extended property object by name
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ExtendedProperty GetExtendedProperty(this Type type, string name)
        {
            return type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Get an extended field object by name
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ExtendedField GetExtendedField(this Type type, string name)
        {
            return type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}
