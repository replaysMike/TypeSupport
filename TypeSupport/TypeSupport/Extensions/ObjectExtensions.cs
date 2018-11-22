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
        public static ICollection<PropertyInfo> GetProperties(this object obj)
        {
            var t = obj.GetType();
            return t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Get all of the fields of an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="includeAutoPropertyBackingFields">True to include the compiler generated backing fields for auto-property getters/setters</param>
        /// <returns></returns>
        public static ICollection<FieldInfo> GetFields(this object obj, bool includeAutoPropertyBackingFields = false)
        {
            var t = obj.GetType();
            var allFields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (!includeAutoPropertyBackingFields)
            {
                var allFieldsExcludingAutoPropertyFields = allFields.Where(x => !x.Name.Contains("k__BackingField")).ToList();
                return allFieldsExcludingAutoPropertyFields;
            }
            return allFields;
        }

        /// <summary>
        /// Get a property from an object instance
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static PropertyInfo GetProperty(this object obj, string name)
        {
            var t = obj.GetType();
            return t.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Get a field from an object instance
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static FieldInfo GetField(this object obj, string name)
        {
            var t = obj.GetType();
            return t.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Set value of a property on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="valueToSet"></param>
        public static void SetPropertyValue(this object obj, string propertyName, object valueToSet)
        {
            var type = obj.GetType();
            var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property != null)
            {
                if (property.SetMethod != null)
                    property.SetValue(obj, valueToSet);
                else
                {
                    // if this is an auto-property with a backing field, set it
                    var field = obj.GetType().GetField($"<{property.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (field != null)
                        field.SetValue(obj, valueToSet);
                    else
                        throw new ArgumentException($"Property '{propertyName}' does not exist.");
                }
            }
            else
                throw new ArgumentException($"Property '{propertyName}' does not exist.");
        }

        /// <summary>
        /// Set the value of a field on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldName"></param>
        /// <param name="valueToSet"></param>
        public static void SetFieldValue(this object obj, string fieldName, object valueToSet)
        {
            var type = obj.GetType();
            var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
                field.SetValue(obj, valueToSet);
            else
                throw new ArgumentException($"Field '{fieldName}' does not exist.");
        }

        /// <summary>
        /// Set the value of a property on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <param name="valueToSet"></param>
        /// <param name="path"></param>
        public static void SetPropertyValue(this object obj, PropertyInfo property, object valueToSet, string path)
        {
            try
            {
                if (property.SetMethod != null)
                    property.SetValue(obj, valueToSet);
                else
                {
                    // if this is an auto-property with a backing field, set it
                    var field = obj.GetType().GetField($"<{property.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (field != null)
                        field.SetValue(obj, valueToSet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Set the value of a field on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="field"></param>
        /// <param name="valueToSet"></param>
        /// <param name="path"></param>
        public static void SetFieldValue(this object obj, FieldInfo field, object valueToSet, string path)
        {
            try
            {
                field.SetValue(obj, valueToSet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
