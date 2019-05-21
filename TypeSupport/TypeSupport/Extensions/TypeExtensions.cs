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
            if (type == null)
                return new List<ExtendedProperty>();
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var allProperties = type.GetProperties(flags);
            IEnumerable<PropertyInfo> returnProperties = allProperties;

            if (options.HasFlag(PropertyOptions.Public))
                returnProperties = returnProperties.Where(x => x.GetGetMethod(true).IsPublic && x.GetSetMethod(true).IsPublic);
            if (options.HasFlag(PropertyOptions.Private))
                returnProperties = returnProperties.Where(x => x.GetGetMethod(true).IsPrivate && x.GetSetMethod(true).IsPrivate);
#if FEATURE_CUSTOM_ATTRIBUTES
            if (options.HasFlag(PropertyOptions.HasSetter))
                returnProperties = returnProperties.Where(x => x.SetMethod != null);
            if (options.HasFlag(PropertyOptions.HasGetter))
                returnProperties = returnProperties.Where(x => x.GetMethod != null);
#else
            if (options.HasFlag(PropertyOptions.HasSetter))
                returnProperties = returnProperties.Where(x => x.GetSetMethod(true) != null);
            if (options.HasFlag(PropertyOptions.HasGetter))
                returnProperties = returnProperties.Where(x => x.GetGetMethod(true) != null);
#endif
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
                // technically init only are writable when constructed
                // returnFields = returnFields.Where(x => !x.FieldInfo.IsInitOnly);
            }

            if (options.HasFlag(FieldOptions.Public))
                returnFields = returnFields.Where(x => x.FieldInfo.IsPublic);
            if (options.HasFlag(FieldOptions.Private))
                returnFields = returnFields.Where(x => x.FieldInfo.IsPrivate);
            if (options.HasFlag(FieldOptions.Static))
                returnFields = returnFields.Where(x => x.FieldInfo.IsStatic);
            if (options.HasFlag(FieldOptions.BackingFields))
                returnFields = allFields.Where(x => x.IsBackingField);
            if (options.HasFlag(FieldOptions.Constants))
                returnFields = returnFields.Where(x => x.FieldInfo.IsLiteral);
            return returnFields.Select(x => (ExtendedField)x).ToList();
        }

        /// <summary>
        /// Get all of the fields of an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="includeAutoPropertyBackingFields">True to include the compiler generated backing fields for auto-property getters/setters</param>
        /// <returns></returns>
        public static ICollection<ExtendedMethod> GetMethods(this Type type, MethodOptions options)
        {
            if (type == null)
                return new List<ExtendedMethod>();
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static;
            var allMethods = type.GetMethods(flags)
                .Select(x => new ExtendedMethod(x, type))
                .Concat(GetMethods(type.BaseType, options));
            IEnumerable<ExtendedMethod> returnMethods = allMethods;

            if (options.HasFlag(MethodOptions.Public))
                returnMethods = returnMethods.Where(x => x.IsPublic);
            if (options.HasFlag(MethodOptions.Private))
                returnMethods = returnMethods.Where(x => x.IsPrivate);
            if (options.HasFlag(MethodOptions.Static))
                returnMethods = returnMethods.Where(x => x.IsStatic);
            if (options.HasFlag(MethodOptions.Overridden))
                returnMethods = allMethods.Where(x => x.IsOverride);
            if (options.HasFlag(MethodOptions.Virtual))
                returnMethods = returnMethods.Where(x => x.IsVirtual);
            if (options.HasFlag(MethodOptions.Constructor))
                returnMethods = returnMethods.Where(x => x.IsConstructor);
            return returnMethods.Select(x => (ExtendedMethod)x).ToList();
        }

        /// <summary>
        /// Check if a type contains a property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Property name</param>
        /// <returns></returns>
        public static bool ContainsProperty(this Type type, string name)
        {
            return type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) != null;
        }

        /// <summary>
        /// Check if a type contains a field
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Field name</param>
        /// <returns></returns>
        public static bool ContainsField(this Type type, string name)
        {
            return type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) != null;
        }

        /// <summary>
        /// Get a list of all constructors of a type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ICollection<ConstructorInfo> GetConstructors(this Type type, ConstructorOptions options)
        {
            if (type == null)
                return new List<ConstructorInfo>();
            var allConstructors = type.GetConstructors()
                .Concat(GetConstructors(type.BaseType != typeof(object) ? type.BaseType : null, options));
            IEnumerable<ConstructorInfo> returnFields = allConstructors;
            if (options.HasFlag(ConstructorOptions.EmptyConstructor))
                returnFields = returnFields.Where(x => x.GetParameters().Any() == false);
            if (options.HasFlag(ConstructorOptions.NonEmptyConstructor))
                returnFields = returnFields.Where(x => x.GetParameters().Any() == true);

            return returnFields.ToList();
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
        /// Get an extended property object by name
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="declaringType">The declaring type the property belongs to</param>
        /// <returns></returns>
        public static ExtendedProperty GetExtendedProperty(this Type type, string name, Type declaringType)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return properties.Single(x => x.Name.Equals(name) && x.DeclaringType.Equals(declaringType));
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

        /// <summary>
        /// Get an extended field object by name
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="declaringType">The delcaring type the field belongs to</param>
        /// <returns></returns>
        public static ExtendedField GetExtendedField(this Type type, string name, Type declaringType)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return fields.Single(x => x.Name.Equals(name) && x.DeclaringType.Equals(declaringType));
        }
    }
}
