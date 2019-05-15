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
        /// <summary>
        /// Get all of the properties of an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ICollection<ExtendedProperty> GetProperties(this object obj, PropertyOptions options)
        {
            return obj.GetType().GetProperties(options);
        }

        /// <summary>
        /// Get all of the fields of an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="includeAutoPropertyBackingFields">True to include the compiler generated backing fields for auto-property getters/setters</param>
        /// <returns></returns>
        public static ICollection<ExtendedField> GetFields(this object obj, FieldOptions options)
        {
            return obj.GetType().GetFields(options);
        }

        /// <summary>
        /// Get a property from an object instance
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldName">Field name</param>
        /// <returns></returns>
        public static PropertyInfo GetProperty(this object obj, string name)
        {
            var t = obj.GetType();
            return t.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Get a property from an object instance and specify the derived type to match
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldName">Field name</param>
        /// <param name="derivedType">Derived type</param>
        /// <returns></returns>
        public static PropertyInfo GetProperty(this object obj, string name, Type derivedType)
        {
            var t = obj.GetType();
            var properties = t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var val = properties
                .FirstOrDefault(p => p.Name.Equals(name) && p.PropertyType.Equals(derivedType));
            if (val == null)
            {
                val = properties
                    .Single(p => p.Name.Equals(name) && p.DeclaringType.Equals(derivedType));
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
        {
            var t = obj.GetType();
            return t.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Check if an object contains a property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Property name</param>
        /// <returns></returns>
        public static bool ContainsProperty(this object obj, string name)
        {
            return GetProperty(obj, name) != null;
        }

        /// <summary>
        /// Check if an object contains a field
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Field name</param>
        /// <returns></returns>
        public static bool ContainsField(this object obj, string name)
        {
            return GetField(obj, name) != null;
        }

        /// <summary>
        /// Set value of a property on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Property name</param>
        /// <param name="valueToSet"></param>
        public static void SetPropertyValue(this object obj, string name, object valueToSet)
        {
            var type = obj.GetType();
            var property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property != null)
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
                        throw new ArgumentException($"Property '{name}' does not exist.");
                }
            }
            else
                throw new ArgumentException($"Property '{name}' does not exist.");
        }

        /// <summary>
        /// Set the value of a field on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">Field name</param>
        /// <param name="valueToSet"></param>
        public static void SetFieldValue(this object obj, string name, object valueToSet)
        {
            var type = obj.GetType();
            var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
                field.SetValue(obj, valueToSet);
            else
                throw new ArgumentException($"Field '{name}' does not exist.");
        }

        /// <summary>
        /// Set the value of a property on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <param name="valueToSet"></param>
        public static void SetPropertyValue(this object obj, PropertyInfo property, object valueToSet)
        {
            try
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
            catch (Exception)
            {
                throw;
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
            var type = obj.GetType();
            var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (property == null)
                throw new InvalidOperationException($"Unknown property name: {name}");

            try
            {
#if FEATURE_SETVALUE
                return property.GetValue(obj);
#else
                return property.GetValue(obj, null);
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
        public static T GetPropertyValue<T>(this object obj, string name)
        {
            var type = obj.GetType();
            var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            if (property == null)
            {
                throw new InvalidOperationException($"Unknown property name: '{name}'");
            }

            if (property.PropertyType != typeof(T))
            {
                throw new InvalidOperationException($"Specified property '{name}' is of a different type '{typeof(T)}'");
            }

            try
            {
#if FEATURE_SETVALUE
                return (T)property.GetValue(obj);
#else
                return (T)property.GetValue(obj, null);
#endif                
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
        /// <param name="field"></param>
        public static object GetFieldValue(this object obj, ExtendedField field)
        {
            try
            {
                return field.FieldInfo.GetValue(obj);
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
        /// <param name="field"></param>
        public static T GetFieldValue<T>(this object obj, ExtendedField field)
        {
            try
            {
                return (T)field.FieldInfo.GetValue(obj);
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
        public static object GetFieldValue(this object obj, string name)
        {
            var type = obj.GetType();
            var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
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
            var type = obj.GetType();
            var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
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
