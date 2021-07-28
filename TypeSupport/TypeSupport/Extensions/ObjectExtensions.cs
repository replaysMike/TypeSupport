using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TypeSupport.Extensions
{
    /// <summary>
    /// Object TypeSupport extensions
    /// </summary>
    public static class ObjectExtensions
    {
        private const BindingFlags DefaultPropertyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        private const BindingFlags DefaultFieldBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        /// <summary>
        /// Get the extended type for a Type
        /// </summary>
        /// <param name="instanceOrType"></param>
        /// <returns></returns>
        public static ExtendedType GetExtendedType(this object instanceOrType)
            => GetExtendedType(instanceOrType, TypeSupportOptions.All);

        /// <summary>
        /// Get the extended type for an instance
        /// </summary>
        /// <param name="instanceOrType"></param>
        /// <param name="options">The type support inspection options</param>
        /// <returns></returns>
        public static ExtendedType GetExtendedType(this object instanceOrType, TypeSupportOptions options)
        {
            if (instanceOrType is null)
                return null;
            if (object.ReferenceEquals(instanceOrType, typeof(ExtendedType)))
                return (ExtendedType)instanceOrType;
            if (object.ReferenceEquals(instanceOrType, typeof(Type)))
                return ExtendedTypeCache.GetOrCreate(instanceOrType as Type, options);

            return ExtendedTypeCache.GetOrCreate(instanceOrType.GetType(), options);
        }

        /// <summary>
        /// Get all of the properties of an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ICollection<ExtendedProperty> GetProperties(this object obj, PropertyOptions options)
            => obj.GetType().GetProperties(options);

        /// <summary>
        /// Get all of the fields of an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ICollection<ExtendedField> GetFields(this object obj, FieldOptions options)
            => obj.GetType().GetFields(options);

        /// <summary>
        /// Get all of the methods of an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ICollection<ExtendedMethod> GetMethods(this object obj, MethodOptions options)
            => obj.GetType().GetMethods(options);

        /// <summary>
        /// Get a property from an object instance
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Property name</param>
        /// <returns></returns>
        public static PropertyInfo GetProperty(this object obj, string name)
            => GetProperty(obj, name, false);

        /// <summary>
        /// Get a property from an object instance
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Property name</param>
        /// <param name="searchBaseTypes">True to search for the property name in the object's base types</param>
        /// <returns></returns>
        public static PropertyInfo GetProperty(this object obj, string name, bool searchBaseTypes)
            => GetProperty(obj, name, DefaultPropertyBindingFlags, searchBaseTypes);

        /// <summary>
        /// Get a property from an object instance
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Property name</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more BindingFlags that specify how the search is conducted.</param>
        /// <param name="searchBaseTypes">True to search for the property name in the object's base types</param>
        /// <returns></returns>
        public static PropertyInfo GetProperty(this object obj, string name, BindingFlags bindingFlags, bool searchBaseTypes)
        {
            var t = obj.GetType();
            var property = t.GetProperty(name, bindingFlags);

            if (!searchBaseTypes)
                return property;
            var baseType = t.BaseType;
            while (property == null && baseType != null)
            {
                property = GetPropertyFromType(baseType, name, bindingFlags);
                if (property == null)
                    baseType = baseType.BaseType;
            }

            return property;
        }

        private static PropertyInfo GetPropertyFromType(Type type, string name, BindingFlags bindingFlags)
            => type.GetProperty(name, bindingFlags);

        /// <summary>
        /// Get a property from an object instance and specify the derived type to match
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Field name</param>
        /// <param name="derivedType">The type that defines the named property, or the exact type of the property</param>
        /// <returns></returns>
        public static PropertyInfo GetProperty(this object obj, string name, Type derivedType)
            => GetProperty(obj, name, derivedType, DefaultPropertyBindingFlags);

        /// <summary>
        /// Get a property from an object instance and specify the derived type to match
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Field name</param>
        /// <param name="derivedType">The type that defines the named property, or the exact type of the property</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more BindingFlags that specify how the search is conducted.</param>
        /// <returns></returns>
        public static PropertyInfo GetProperty(this object obj, string name, Type derivedType, BindingFlags bindingFlags)
        {
            var t = obj.GetType();
            var properties = t.GetProperties(bindingFlags);
            var val = properties
                .FirstOrDefault(p => p.Name.Equals(name) && p.PropertyType.Equals(derivedType));
            if (val == null)
            {
                val = properties
                    .FirstOrDefault(p => p.Name.Equals(name) && p.DeclaringType.Equals(derivedType));
            }
            return val;
        }

        /// <summary>
        /// Get a field from an object instance
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Field name</param>
        /// <returns></returns>
        public static FieldInfo GetField(this object obj, string name)
            => GetField(obj, name, false);

        /// <summary>
        /// Get a field from an object instance
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Field name</param>
        /// <param name="searchBaseTypes">True to search for the field name in the object's base types</param>
        /// <returns></returns>
        public static FieldInfo GetField(this object obj, string name, bool searchBaseTypes)
            => GetField(obj, name, DefaultFieldBindingFlags, searchBaseTypes);

        /// <summary>
        /// Get a field from an object instance
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Field name</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more BindingFlags that specify how the search is conducted.</param>
        /// <returns></returns>
        public static FieldInfo GetField(this object obj, string name, BindingFlags bindingFlags)
            => GetField(obj, name, bindingFlags, false);

        /// <summary>
        /// Get a field from an object instance
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Field name</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more BindingFlags that specify how the search is conducted.</param>
        /// <param name="searchBaseTypes">True to search for the field name in the object's base types</param>
        /// <returns></returns>
        public static FieldInfo GetField(this object obj, string name, BindingFlags bindingFlags, bool searchBaseTypes)
        {
            var t = obj.GetType();
            var field = t.GetField(name, bindingFlags);

            if (!searchBaseTypes)
                return field;
            var baseType = t.BaseType;
            while (field == null && baseType != null)
            {
                field = GetFieldFromType(baseType, name, bindingFlags);
                if (field == null)
                    baseType = baseType.BaseType;
            }

            return field;
        }

        private static FieldInfo GetFieldFromType(Type type, string name, BindingFlags bindingFlags)
            => type.GetField(name, bindingFlags);

        /// <summary>
        /// Get a field from an object instance
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Field name</param>
        /// <param name="derivedType">The type that defines the named field, or the exact type of the field</param>
        /// <returns></returns>
        public static FieldInfo GetField(this object obj, string name, Type derivedType)
        {
            var t = obj.GetType();
            var fields = t.GetFields(DefaultFieldBindingFlags);
            var val = fields
                .FirstOrDefault(p => p.Name.Equals(name) && p.FieldType.Equals(derivedType));
            if (val == null)
            {
                val = fields
                    .FirstOrDefault(p => p.Name.Equals(name) && p.DeclaringType.Equals(derivedType));
            }
            return val;
        }

        /// <summary>
        /// Check if an object contains a property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Property name</param>
        /// <returns></returns>
        public static bool ContainsProperty(this object obj, string name)
            => GetProperty(obj, name) != null;

        /// <summary>
        /// Check if an object contains a property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Property name</param>
        /// <param name="derivedType">The type that defines the named property, or the exact type of the property</param>
        /// <returns></returns>
        public static bool ContainsProperty(this object obj, string name, Type derivedType)
            => GetProperty(obj, name, derivedType) != null;

        /// <summary>
        /// Check if an object contains a field
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Field name</param>
        /// <returns></returns>
        public static bool ContainsField(this object obj, string name)
            => GetField(obj, name) != null;

        /// <summary>
        /// Check if an object contains a field
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Field name</param>
        /// <param name="derivedType">The type that defines the named field, or the exact type of the field</param>
        /// <returns></returns>
        public static bool ContainsField(this object obj, string name, Type derivedType)
            => GetField(obj, name, derivedType) != null;

        /// <summary>
        /// Set value of a property on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Property name</param>
        /// <param name="valueToSet">The value to set</param>
        public static void SetPropertyValue(this object obj, string name, object valueToSet)
        {
            var property = GetProperty(obj, name);
            if (property != null)
            {
                SetPropertyValueInternal(obj, property, valueToSet);
            }
            else
                throw new ArgumentException($"Property '{name}' does not exist.");
        }

        /// <summary>
        /// Set value of a property on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Property name</param>
        /// <param name="derivedType">The type that defines the named property, or the exact type of the property</param>
        /// <param name="valueToSet">The value to set</param>
        public static void SetPropertyValue(this object obj, string name, Type derivedType, object valueToSet)
        {
            var property = GetProperty(obj, name, derivedType);
            if (property != null)
            {
                SetPropertyValueInternal(obj, property, valueToSet);
            }
            else
                throw new ArgumentException($"Property '{name}' does not exist.");
        }

        private static void SetPropertyValueInternal(object obj, PropertyInfo property, object valueToSet)
        {
#if FEATURE_GETMETHOD
            if (property.SetMethod != null)
#else
                if (property.GetSetMethod(true) != null)
#endif
            {
                var indexParameters = property.GetIndexParameters();
                if (!indexParameters.Any())
#if FEATURE_SETVALUE
                    property.SetValue(obj, valueToSet);
#else
                        property.SetValue(obj, valueToSet, null);
#endif
            }
            else
            {
                // if this is an auto-property with a backing field, set it
                var field = obj.GetType().GetField($"<{property.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                    field.SetValue(obj, valueToSet);
                else
                    throw new ArgumentException($"Property '{property.Name}' does not exist.");
            }
        }

        /// <summary>
        /// Set the value of a field on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Field name</param>
        /// <param name="valueToSet">The value to set</param>
        public static void SetFieldValue(this object obj, string name, object valueToSet)
        {
            var field = GetField(obj, name);
            if (field != null)
            {
                SetFieldValueInternal(obj, field, valueToSet);
            }
            else
                throw new ArgumentException($"Field '{name}' does not exist.");
        }

        /// <summary>
        /// Set the value of a field on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Field name</param>
        /// <param name="derivedType">The type that defines the named field, or the exact type of the field</param>
        /// <param name="valueToSet">The value to set</param>
        public static void SetFieldValue(this object obj, string name, Type derivedType, object valueToSet)
        {
            var field = GetField(obj, name, derivedType);
            if (field != null)
            {
                SetFieldValueInternal(obj, field, valueToSet);
            }
            else
                throw new ArgumentException($"Field '{name}' does not exist.");
        }

        private static void SetFieldValueInternal(object obj, FieldInfo field, object valueToSet)
            => field.SetValue(obj, valueToSet);

        /// <summary>
        /// Set the value of a property on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <param name="valueToSet"></param>
        public static void SetPropertyValue(this object obj, PropertyInfo property, object valueToSet)
        {
#if FEATURE_GETMETHOD
            if (property.SetMethod != null)
#else
                if (property.GetSetMethod(true) != null)
#endif
            {
                var indexParameters = property.GetIndexParameters();
                if (!indexParameters.Any())
#if FEATURE_SETVALUE
                    property.SetValue(obj, valueToSet);
#else
                        property.SetValue(obj, valueToSet, null);
#endif
            }
            else
            {
                // if this is an auto-property with a backing field, set it
                var field = obj.GetType().GetField($"<{property.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                    field.SetValue(obj, valueToSet);
            }
        }

        /// <summary>
        /// Set the value of a field on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="field"></param>
        /// <param name="valueToSet"></param>
        public static void SetFieldValue(this object obj, FieldInfo field, object valueToSet)
        {
            try
            {
                field.SetValue(obj, valueToSet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the value of a property on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        public static object GetPropertyValue(this object obj, ExtendedProperty property)
        {
            try
            {
#if FEATURE_SETVALUE
                return property.PropertyInfo.GetValue(obj);
#else
                return property.PropertyInfo.GetValue(obj, null);
#endif                
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the value of a property on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        public static T GetPropertyValue<T>(this object obj, ExtendedProperty property)
        {
            try
            {
#if FEATURE_SETVALUE
                return (T)property.PropertyInfo.GetValue(obj);
#else
                return (T)property.PropertyInfo.GetValue(obj, null);
#endif                
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the value of a property on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Name of property</param>
        public static object GetPropertyValue(this object obj, string name)
        {
            var property = GetProperty(obj, name);
            return GetPropertyValueInternal(obj, name, property);
        }

        /// <summary>
        /// Get the value of a property on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Name of property</param>
        /// <param name="derivedType">The type that defines the named property, or the exact type of the property</param>
        public static object GetPropertyValue(this object obj, string name, Type derivedType)
        {
            var property = GetProperty(obj, name, derivedType);
            return GetPropertyValueInternal(obj, name, property);
        }

        private static object GetPropertyValueInternal(object obj, string name, PropertyInfo property)
        {
            if (property == null)
                throw new InvalidOperationException($"Unknown property name: {name}");

#if FEATURE_SETVALUE
            return property.GetValue(obj);
#else
                return property.GetValue(obj, null);
#endif                
        }

        /// <summary>
        /// Get the value of a property on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Name of property</param>
        public static T GetPropertyValue<T>(this object obj, string name)
        {
            var property = GetProperty(obj, name);
            return GetPropertyValueInternal<T>(obj, property, name);
        }

        /// <summary>
        /// Get the value of a property on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Name of property</param>
        /// <param name="derivedType">The type that defines the named property, or the exact type of the property</param>
        public static T GetPropertyValue<T>(this object obj, string name, Type derivedType)
        {
            var property = GetProperty(obj, name, derivedType);
            return GetPropertyValueInternal<T>(obj, property, name);
        }

        private static T GetPropertyValueInternal<T>(object obj, PropertyInfo property, string name)
        {
            if (property == null)
                throw new InvalidOperationException($"Unknown property name: '{name}'");

            if (property.PropertyType != typeof(T))
                throw new InvalidOperationException($"Specified property '{name}' is of a different type '{typeof(T)}'");

#if FEATURE_SETVALUE
            return (T)property.GetValue(obj);
#else
                return (T)property.GetValue(obj, null);
#endif                
        }

        /// <summary>
        /// Get the value of a field on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="field"></param>
        public static object GetFieldValue(this object obj, ExtendedField field)
            => field.FieldInfo.GetValue(obj);

        /// <summary>
        /// Get the value of a field on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="field"></param>
        public static T GetFieldValue<T>(this object obj, ExtendedField field)
            => (T)field.FieldInfo.GetValue(obj);

        /// <summary>
        /// Get the value of a field on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Name of field</param>
        public static object GetFieldValue(this object obj, string name)
        {
            var field = GetField(obj, name);
            return GetFieldValueInternal(obj, field, name);
        }

        /// <summary>
        /// Get the value of a field on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Name of field</param>
        /// <param name="derivedType">The type that defines the named field, or the exact type of the field</param>
        public static object GetFieldValue(this object obj, string name, Type derivedType)
        {
            var field = GetField(obj, name, derivedType);
            return GetFieldValueInternal(obj, field, name);
        }

        private static object GetFieldValueInternal(object obj, FieldInfo field, string name)
        {
            if (field == null)
                throw new InvalidOperationException($"Unknown field name: {name}");
            try
            {
                return field.GetValue(obj);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the value of a field on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Name of field</param>
        public static T GetFieldValue<T>(this object obj, string name)
        {
            var field = GetField(obj, name);
            return GetFieldValueInternal<T>(obj, field, name);
        }

        /// <summary>
        /// Get the value of a field on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Name of field</param>
        /// <param name="derivedType">The type that defines the named field, or the exact type of the field</param>
        public static T GetFieldValue<T>(this object obj, string name, Type derivedType)
        {
            var field = GetField(obj, name, derivedType);
            return GetFieldValueInternal<T>(obj, field, name);
        }

        private static T GetFieldValueInternal<T>(object obj, FieldInfo field, string name)
        {
            if (field == null)
                throw new InvalidOperationException($"Unknown field name: '{name}'");
            if (field.FieldType != typeof(T))
                throw new InvalidOperationException($"Specified field '{name}' is of a different type '{typeof(T)}'");
            try
            {
                return (T)field.GetValue(obj);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
