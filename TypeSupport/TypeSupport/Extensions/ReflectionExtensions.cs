using System.Reflection;

namespace TypeSupport.Extensions
{
    /// <summary>
    /// Reflection extensions
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Get the extended field
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public static ExtendedField ExtendedField(this FieldInfo fieldInfo)
        {
            return new ExtendedField(fieldInfo);
        }

        /// <summary>
        /// Get the extended property
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static ExtendedProperty ExtendedProperty(this PropertyInfo propertyInfo)
        {
            return new ExtendedProperty(propertyInfo);
        }
    }
}
