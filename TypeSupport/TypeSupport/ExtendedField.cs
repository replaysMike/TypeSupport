using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TypeSupport.Extensions;

namespace TypeSupport
{
    /// <summary>
    /// Discovers the attributes of a field and provides access to field metadata
    /// </summary>
    public class ExtendedField : IAttributeInspection
    {
        private readonly FieldInfo _fieldInfo;
        private readonly TypeSupportOptions _typeSupportOptions;

        /// <summary>
        /// Original FieldInfo of the field
        /// </summary>
        public FieldInfo FieldInfo => _fieldInfo;

        /// <summary>
        /// Gets the name of the current member
        /// </summary>
        public string Name => _fieldInfo.Name;

        /// <summary>
        /// Gets the type of this field object
        /// </summary>
        public ExtendedType Type => _fieldInfo.FieldType?.GetExtendedType(_typeSupportOptions);

        /// <summary>
        /// Gets the base type of this field object
        /// </summary>
        public ExtendedType BaseType => _fieldInfo.FieldType.BaseType?.GetExtendedType(_typeSupportOptions);

        /// <summary>
        /// Gets the class object that was used to obtain this instance of MemberInfo
        /// </summary>
        public ExtendedType ReflectedType => _fieldInfo.ReflectedType?.GetExtendedType(_typeSupportOptions);

#if FEATURE_CUSTOM_ATTRIBUTES
        /// <summary>
        /// Gets a collection that contains this member's custom attributes
        /// </summary>
        public IEnumerable<CustomAttributeData> CustomAttributes => _fieldInfo.CustomAttributes;
#else
        /// <summary>
        /// Gets a collection that contains this member's custom attributes
        /// </summary>
        public IEnumerable<CustomAttributeData> CustomAttributes => _fieldInfo.GetCustomAttributesData();
#endif

        /// <summary>
        /// True if this field backs an auto-property
        /// </summary>
        public bool IsBackingField { get; private set; }

        /// <summary>
        /// True if field is defined as static
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// True if field is defined as a constant
        /// </summary>
        public bool IsConstant { get; set; }

        /// <summary>
        /// True if field is defined as private
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// True if field is defined as public
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// True if field is defined as protected
        /// </summary>
        public bool IsProtected { get; set; }

        /// <summary>
        /// True if field is defined as internal
        /// </summary>
        public bool IsInternal { get; set; }

        /// <summary>
        /// For backing fields, the property name it stores data for
        /// </summary>
        public string BackedPropertyName { get; private set; }

        /// <summary>
        /// For backing fields, the property it stores data for
        /// </summary>
        public ExtendedProperty BackedProperty { get; private set; }

        /// <summary>
        /// Create an extended field
        /// </summary>
        /// <param name="fieldInfo"></param>
        public ExtendedField(FieldInfo fieldInfo) : this(fieldInfo, TypeSupportOptions.All) { }

        /// <summary>
        /// Create an extended field
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="typeSupportOptions">The type support options to use for type inspection</param>
        public ExtendedField(FieldInfo fieldInfo, TypeSupportOptions typeSupportOptions)
        {
            _fieldInfo = fieldInfo;
            _typeSupportOptions = typeSupportOptions;

            IsStatic = _fieldInfo.IsStatic;
            IsPrivate = _fieldInfo.IsPrivate;
            IsPublic = _fieldInfo.IsPublic;
            IsConstant = _fieldInfo.IsLiteral;
            IsProtected = _fieldInfo.IsFamily;
            IsInternal = _fieldInfo.IsAssembly;

            var name = fieldInfo.Name;
            if (name.Contains("k__BackingField") || name.StartsWith("<"))
            {
                IsBackingField = true;
                var i = name.IndexOf("<");
                var end = name.LastIndexOf(">");

                BackedPropertyName = name.Substring(i + 1, end - (i + 1));
                BackedProperty = ReflectedType.GetExtendedProperty(BackedPropertyName, fieldInfo.DeclaringType);
            }
        }

        public bool HasAttribute<TAttribute>() where TAttribute : class => Attribute.GetCustomAttribute(this, typeof(TAttribute)) != null;

        public bool HasAttribute(Type attributeType) => Attribute.GetCustomAttribute(this, attributeType) != null;

        public TAttribute GetAttribute<TAttribute>() where TAttribute : class => Attribute.GetCustomAttribute(this, typeof(TAttribute)) as TAttribute;

        public Attribute GetAttribute(Type attributeType) => Attribute.GetCustomAttribute(this, attributeType);

        /// <summary>
        /// Get all custom attributes on field
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Attribute> GetAttributes()
        {
            var attributes = _fieldInfo.GetCustomAttributes(true);
            return attributes.Cast<Attribute>();
        }

        /// <summary>
        /// Get all custom attributes on field
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TAttribute> GetAttributes<TAttribute>()
            where TAttribute : Attribute
        {
            var attributes = _fieldInfo.GetCustomAttributes(true);
            return attributes.OfType<TAttribute>().Cast<TAttribute>();
        }

        public static implicit operator ExtendedField(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                return null;
            return new ExtendedField(fieldInfo);
        }

        public static implicit operator FieldInfo(ExtendedField extendedField)
        {
            if (extendedField == null)
                return null;
            return extendedField._fieldInfo;
        }

        public override string ToString() => $"{ReflectedType}.{Name}";
    }
}
