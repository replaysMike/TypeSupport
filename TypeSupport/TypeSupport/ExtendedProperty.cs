using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeSupport.Extensions;

namespace TypeSupport
{
    /// <summary>
    /// Discovers the attributes of a property and provides access to property metadata
    /// </summary>
    public class ExtendedProperty : IAttributeInspection
    {
        private readonly PropertyInfo _propertyInfo;
        private readonly TypeSupportOptions _typeSupportOptions;

        /// <summary>
        /// Original PropertyInfo of the property
        /// </summary>
        public PropertyInfo PropertyInfo => _propertyInfo;

        /// <summary>
        /// Gets the name of the current member
        /// </summary>
        public string Name => _propertyInfo.Name;

        /// <summary>
        /// Gets the type of this property object
        /// </summary>
        public ExtendedType Type => _propertyInfo.PropertyType?.GetExtendedType(_typeSupportOptions);

        /// <summary>
        /// Gets the base type of this property object
        /// </summary>
        public ExtendedType BaseType => _propertyInfo.PropertyType.BaseType?.GetExtendedType(_typeSupportOptions);

        /// <summary>
        /// Gets the class object that was used to obtain this instance of MemberInfo
        /// </summary>
        public ExtendedType ReflectedType => _propertyInfo.ReflectedType?.GetExtendedType(_typeSupportOptions);

#if FEATURE_CUSTOM_ATTRIBUTES
        /// <summary>
        /// Gets a collection that contains this member's custom attributes
        /// </summary>
        public IEnumerable<CustomAttributeData> CustomAttributes => _propertyInfo.CustomAttributes;
#else
        public IEnumerable<CustomAttributeData> CustomAttributes => _propertyInfo.GetCustomAttributesData();
#endif

        public IEnumerable<Attribute> Attributes => GetAttributes();

        /// <summary>
        /// True if an auto-backed property
        /// </summary>
        public bool IsAutoProperty { get; }

        /// <summary>
        /// True if property is defined as static
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// True if property is defined as virtual
        /// </summary>
        public bool IsVirtual { get; set; }

        /// <summary>
        /// True if property is defined as abstract
        /// </summary>
        public bool IsAbstract { get; set; }

        /// <summary>
        /// True if property is defined as final
        /// </summary>
        public bool IsFinal { get; set; }

        /// <summary>
        /// True if property is defined as private
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// True if property is defined as public
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// True if property is defined as protected
        /// </summary>
        public bool IsProtected { get; set; }

        /// <summary>
        /// True if property is defined as internal
        /// </summary>
        public bool IsInternal { get; set; }

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
        public ExtendedProperty(PropertyInfo propertyInfo) : this(propertyInfo, TypeSupportOptions.All) { }

        /// <summary>
        /// Create an extended property
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="typeSupportOptions">The type support options to use for type inspection</param>
        public ExtendedProperty(PropertyInfo propertyInfo, TypeSupportOptions typeSupportOptions)
        {
            _propertyInfo = propertyInfo;
            _typeSupportOptions = typeSupportOptions;

            if (HasGetMethod)
            {
                IsStatic = GetMethod.IsStatic;
                IsVirtual = GetMethod.IsVirtual;
                IsAbstract = GetMethod.IsAbstract;
                IsFinal = GetMethod.IsFinal;
                IsPrivate = GetMethod.IsPrivate;
                IsPublic = GetMethod.IsPublic;
                IsProtected = GetMethod.IsFamily;
                IsInternal = GetMethod.IsAssembly;

                if (GetMethod
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
        }

        public bool HasAttribute<TAttribute>() where TAttribute : class => Attribute.GetCustomAttribute(this, typeof(TAttribute)) != null;

        public bool HasAttribute(Type attributeType) => Attribute.GetCustomAttribute(this, attributeType) != null;

        public TAttribute GetAttribute<TAttribute>() where TAttribute : class => Attribute.GetCustomAttribute(this, typeof(TAttribute)) as TAttribute;

        public Attribute GetAttribute(Type attributeType) => Attribute.GetCustomAttribute(this, attributeType);

        /// <summary>
        /// Get all custom attributes on property
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Attribute> GetAttributes()
        {
            var attributes = _propertyInfo.GetCustomAttributes(true);
            return attributes.Cast<Attribute>();
        }

        /// <summary>
        /// Get all custom attributes on property
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TAttribute> GetAttributes<TAttribute>()
            where TAttribute : Attribute
        {
            var attributes = _propertyInfo.GetCustomAttributes(true);
            return attributes.OfType<TAttribute>().Cast<TAttribute>();
        }

        public static implicit operator ExtendedProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                return null;
            return new ExtendedProperty(propertyInfo);
        }

        public static implicit operator PropertyInfo(ExtendedProperty extendedProperty)
            => extendedProperty?._propertyInfo;

        public override string ToString() => $"{ReflectedType.Name}.{Name}";
    }
}
