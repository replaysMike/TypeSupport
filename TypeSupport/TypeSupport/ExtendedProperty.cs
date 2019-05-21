using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TypeSupport
{
    /// <summary>
    /// Discovers the attributes of a property and provides access to property metadata
    /// </summary>
    public class ExtendedProperty
    {
        private readonly PropertyInfo _propertyInfo;

        public PropertyInfo PropertyInfo
        {
            get
            {
                return _propertyInfo;
            }
        }

        /// <summary>
        /// Gets the name of the current member
        /// </summary>
        public string Name => _propertyInfo.Name;

        /// <summary>
        /// Gets the type of this property object
        /// </summary>
        public Type Type => _propertyInfo.PropertyType;

        /// <summary>
        /// Gets the class object that was used to obtain this instance of MemberInfo
        /// </summary>
        public Type ReflectedType => _propertyInfo.ReflectedType;

#if FEATURE_CUSTOM_ATTRIBUTES
        /// <summary>
        /// Gets a collection that contains this member's custom attributes
        /// </summary>
        public IEnumerable<CustomAttributeData> CustomAttributes => _propertyInfo.CustomAttributes;
#else
        public IEnumerable<CustomAttributeData> CustomAttributes => _propertyInfo.GetCustomAttributesData();
#endif
        /// <summary>
        /// True if an auto-backed property
        /// </summary>
        public bool IsAutoProperty { get; }

        /// <summary>
        /// If an auto-backed property, the name of the field that backs it
        /// </summary>
        public string BackingFieldName { get; }

#if FEATURE_GETMETHOD
        /// <summary>
        /// True if property has a get method
        /// </summary>
        public bool HasGetMethod => _propertyInfo.GetMethod != null;

        /// <summary>
        /// True if property has a set method
        /// </summary>
        public bool HasSetMethod => _propertyInfo.SetMethod != null;
#else
        /// <summary>
        /// True if property has a get method
        /// </summary>
        public bool HasGetMethod => _propertyInfo.GetGetMethod(true) != null;
        
        /// <summary>
        /// True if property has a set method
        /// </summary>
        public bool HasSetMethod => _propertyInfo.GetSetMethod(true) != null;
#endif

        /// <summary>
        /// The getter method for the property
        /// </summary>
        public MethodInfo GetMethod => _propertyInfo.GetGetMethod(true);

        /// <summary>
        /// The setter method for the property
        /// </summary>
        public MethodInfo SetMethod => _propertyInfo.GetSetMethod(true);

        /// <summary>
        /// Create an extended property
        /// </summary>
        /// <param name="propertyInfo"></param>
        public ExtendedProperty(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;

            if (HasGetMethod 
                && GetMethod
                    .GetCustomAttributes(typeof(CompilerGeneratedAttribute), true)
                    .Any()
                && _propertyInfo.DeclaringType
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Any(x => x.Name.Equals($"<{Name}>k__BackingField")))
            {
                IsAutoProperty = true;
                BackingFieldName = $"<{Name}>k__BackingField";
            }
        }

        public static implicit operator ExtendedProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                return null;
            }
            return new ExtendedProperty(propertyInfo);
        }

        public static implicit operator PropertyInfo(ExtendedProperty extendedProperty)
        {
            if (extendedProperty == null)
                return null;
            return extendedProperty._propertyInfo;
        }

        public override string ToString()
        {
            return $"{ReflectedType.Name}.{Name}";
        }
    }
}
