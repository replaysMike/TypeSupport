using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeSupport.Extensions;

[assembly: InternalsVisibleTo("TypeSupport.Tests")]
namespace TypeSupport
{
    /// <summary>
    /// Helper class for getting information about a <see cref="Type"/>
    /// </summary>
    public class ExtendedType : IEquatable<ExtendedType>, IEquatable<Type>, IAttributeInspection, ICloneable
    {
        private readonly TypeInspector _typeInspector;
        private readonly TypeSupportOptions _options;
        private ICollection<ExtendedProperty> _properties;
        private ICollection<ExtendedField> _fields;
        private ICollection<ExtendedMethod> _methods;
        private bool? _hasIndexer;
        private Type _indexerType;
        private Type _indexerReturnType;

        /// <summary>
        /// True if type has an empty constructor defined
        /// </summary>
        public bool HasEmptyConstructor { get; internal set; }

        /// <summary>
        /// True if a base type has an empty constructor defined
        /// </summary>
        public bool BaseHasEmptyConstructor { get; internal set; }

        /// <summary>
        /// True if the type is abstract
        /// </summary>
        public bool IsAbstract { get; internal set; }

        /// <summary>
        /// True if type is an immutable type
        /// </summary>
        public bool IsImmutable { get; internal set; }

        /// <summary>
        /// True if the type implements IEnumerable
        /// </summary>
        public bool IsEnumerable { get; internal set; }

        /// <summary>
        /// True if the type implements ICollection
        /// </summary>
        public bool IsCollection { get; internal set; }

        /// <summary>
        /// True if the type is an array
        /// </summary>
        public bool IsArray { get; internal set; }

        /// <summary>
        /// True if the type implements IDictionary
        /// </summary>
        public bool IsDictionary { get; internal set; }

        /// <summary>
        /// True if the type is a Hashtable
        /// </summary>
        public bool IsHashtable { get; internal set; }

        /// <summary>
        /// True if the type of collection is read only
        /// </summary>
        public bool IsCollectionReadOnly { get; internal set; }

        /// <summary>
        /// True if the type is a key value pair
        /// </summary>
        public bool IsKeyValuePair { get; internal set; }

        /// <summary>
        /// True if the type is a generic type
        /// </summary>
        public bool IsGeneric { get; internal set; }

        /// <summary>
        /// True if the type is a delegate
        /// </summary>
        public bool IsDelegate { get; internal set; }

        /// <summary>
        /// True if the type is an integral value type
        /// </summary>
        public bool IsValueType { get; internal set; }

        /// <summary>
        /// True if the type is a reference type
        /// </summary>
        public bool IsReferenceType { get; internal set; }

        /// <summary>
        /// True if the type is a struct
        /// </summary>
        public bool IsStruct { get; internal set; }

        /// <summary>
        /// True if the type is a primitive type
        /// </summary>
        public bool IsPrimitive { get; internal set; }

        /// <summary>
        /// True if the type is a system enum
        /// </summary>
        public bool IsEnum { get; internal set; }

        /// <summary>
        /// True if the type is a Tuple of any length <see cref="Tuple"/>
        /// </summary>
        public bool IsTuple { get; internal set; }

        /// <summary>
        /// True if they type is a Tuple of any length <see cref="ValueTuple"/>
        /// </summary>
        public bool IsValueTuple { get; internal set; }

        /// <summary>
        /// True if the type is nullable
        /// </summary>
        public bool IsNullable { get; internal set; }

        /// <summary>
        /// True if the type is an interface
        /// </summary>
        public bool IsInterface { get; internal set; }

        /// <summary>
        /// True if the type is serializable
        /// </summary>
        public bool IsSerializable { get; internal set; }

        /// <summary>
        /// True if the type contains an indexer
        /// </summary>
        public bool HasIndexer => _hasIndexer == null ? _typeInspector.InspectHasIndexer() : _hasIndexer.Value;

        /// <summary>
        /// True if the type is an anonymous type
        /// </summary>
        public bool IsAnonymous { get; internal set; }

        /// <summary>
        /// True if the type contains an implementation or is of a concrete type (not: abstract, interfaces, anonymous types, object)
        /// </summary>
        public bool IsConcreteType { get; internal set; }

        /// <summary>
        /// True if the type is a numeric type, which consists of Integral types and floating point types (char/byte/short/int/float/double/decimal/BigInteger etc)
        /// </summary>
        public bool IsNumericType { get; internal set; }

        /// <summary>
        /// For enum types the list of valid values of the Enum
        /// </summary>
        public ICollection<KeyValuePair<object, string>> EnumValues { get; internal set; }

        /// <summary>
        /// The list of types this type inherits from
        /// </summary>
        public ICollection<Type> BaseTypes { get; internal set; }

        /// <summary>
        /// For interface types the list of known concrete types that implement it
        /// </summary>
        public ICollection<Type> KnownConcreteTypes { get; internal set; }

        /// <summary>
        /// The list of attributes defined on the type (concrete types only)
        /// </summary>
        public ICollection<Type> Attributes { get; internal set; }

        /// <summary>
        /// The list of type arguments of a generic type
        /// </summary>
        public ICollection<Type> GenericArgumentTypes { get; internal set; }

        /// <summary>
        /// A list of the type's properties
        /// </summary>
        public ICollection<ExtendedProperty> Properties => _properties ?? _typeInspector.InspectProperties();

        /// <summary>
        /// A list of the type's fields
        /// </summary>
        public ICollection<ExtendedField> Fields => _fields ?? _typeInspector.InspectFields();

        /// <summary>
        /// A list of the type's methods
        /// </summary>
        public ICollection<ExtendedMethod> Methods => _methods ?? _typeInspector.InspectMethods();

        /// <summary>
        /// List of implemented interfaces
        /// </summary>
        public ICollection<Type> Interfaces { get; internal set; }

        /// <summary>
        /// All constructors
        /// </summary>
        public ICollection<ConstructorInfo> Constructors { get; internal set; }

        /// <summary>
        /// All empty constructors
        /// </summary>
        public ICollection<ConstructorInfo> EmptyConstructors { get; internal set; }

        /// <summary>
        /// The type TypeSupport was created from
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// For interface types the concrete type that implements it, <seealso cref="SetConcreteTypeFromInstance(object)"/>
        /// </summary>
        public Type ConcreteType { get; internal set; }

        /// <summary>
        /// For array and enumerable types, the element data type
        /// </summary>
        public Type ElementType { get; internal set; }

        /// <summary>
        /// For array and enumerable types that have a nullable value, the type of the nullable
        /// </summary>
        public Type ElementNullableBaseType { get; internal set; }

        /// <summary>
        /// The declared value type of an enum (one of the following numeric types: sbyte,byte,ushort,short,uint,int,ulong,long)
        /// </summary>
        public Type EnumType { get; internal set; }

        /// <summary>
        /// The declared type of an indexer key
        /// </summary>
        public Type IndexerType => _indexerType ?? _typeInspector.InspectIndexerType();

        /// <summary>
        /// The declared return type of an indexer key
        /// </summary>
        public Type IndexerReturnType => _indexerReturnType ?? _typeInspector.InspectIndexerReturnType();

        /// <summary>
        /// The underlying type of the Type
        /// </summary>
        public Type UnderlyingType { get; internal set; }

        /// <summary>
        /// The base type of a nullable Type
        /// </summary>
        public Type NullableBaseType { get; internal set; }

        /// <summary>
        /// Get the name of the type
        /// </summary>
        public string Name { get { return Type?.Name; } }

        /// <summary>
        /// Get the full name of the type
        /// </summary>
        public string FullName { get { return Type?.FullName; } }

        /// <summary>
        /// Create a new type support
        /// </summary>
        /// <param name="type">The type to analyze</param>
        internal ExtendedType(Type type) : this(type, TypeSupportOptions.All)
        {
        }

        /// <summary>
        /// Create a new type support
        /// </summary>
        /// <param name="type">The type to analyze</param>
        /// <param name="options">The type support inspection options</param>
        internal ExtendedType(Type type, TypeSupportOptions options)
        {
            _options = options;
            Type = type ?? throw new ArgumentNullException();

            // prevent an extended type from enumerating another extended type
            if (object.ReferenceEquals(type, typeof(ExtendedType)))
                throw new InvalidCastException($"You cannot get extended information on a type that is already of ExtendedType");

            Attributes = new List<Type>();
            Interfaces = new List<Type>();
            GenericArgumentTypes = new List<Type>();
            KnownConcreteTypes = new List<Type>();
            EnumValues = new List<KeyValuePair<object, string>>();
            Constructors = new List<ConstructorInfo>();
            EmptyConstructors = new List<ConstructorInfo>();

            // inspect the type with the given options
            _typeInspector = new TypeInspector(this, options);
            _typeInspector.Inspect();
        }

        /// <summary>
        /// Create a new type support
        /// </summary>
        /// <param name="assemblyQualifiedFullName">The full name of the type to create, <see cref="Type.AssemblyQualifiedName"/></param>
        public ExtendedType(string assemblyQualifiedFullName) : this(Type.GetType(assemblyQualifiedFullName), TypeSupportOptions.All)
        {
        }

        /// <summary>
        /// Create a new type support
        /// </summary>
        /// <param name="assemblyQualifiedFullName">The full name of the type to create, <see cref="Type.AssemblyQualifiedName"/></param>
        /// <param name="options">The type support inspection options</param>
        public ExtendedType(string assemblyQualifiedFullName, TypeSupportOptions options) : this(Type.GetType(assemblyQualifiedFullName), options)
        {
        }

        /// <summary>
        /// Refresh type with new options
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public ExtendedType Refresh(TypeSupportOptions options)
        {
            // inspect the type with the given options
            _properties = null;
            _fields = null;
            _methods = null;
            _hasIndexer = null;
            _indexerType = null;
            _indexerReturnType = null;
            _typeInspector.Inspect();
            var isCachingSupported = options.BitwiseHasFlag(TypeSupportOptions.Caching);
            if (isCachingSupported)
                ExtendedTypeCache.CacheType(this, options);
            return this;
        }

        /// <summary>
        /// For interface types, you can provide an object instance to determine a concrete type for it
        /// </summary>
        /// <param name="concreteObject"></param>
        public void SetConcreteTypeFromInstance(object concreteObject)
        {
            if (concreteObject != null)
                ConcreteType = concreteObject.GetType();
        }

        public bool HasAttribute<TAttribute>() where TAttribute : class => Attribute.GetCustomAttribute(Type, typeof(TAttribute)) != null;

        public bool HasAttribute(Type attributeType) => Attribute.GetCustomAttribute(Type, attributeType) != null;

        public TAttribute GetAttribute<TAttribute>() where TAttribute : class => Attribute.GetCustomAttribute(Type, typeof(TAttribute)) as TAttribute;

        public Attribute GetAttribute(Type attributeType) => Attribute.GetCustomAttribute(Type, attributeType);

        /// <summary>
        /// Get all custom attributes on type
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Attribute> GetAttributes()
        {
            var attributes = Type.GetCustomAttributes(true);
            return attributes.Cast<Attribute>();
        }

        /// <summary>
        /// Get all custom attributes on type
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TAttribute> GetAttributes<TAttribute>()
            where TAttribute : Attribute
        {
            var attributes = Type.GetCustomAttributes(true);
            return attributes.OfType<TAttribute>().Cast<TAttribute>();
        }

        /// <summary>
        /// Returns true if the type implements an interface
        /// </summary>
        /// <param name="interfaceType">The interface type to check</param>
        /// <returns></returns>
        public bool Implements(Type interfaceType)
        {
            var interfaceExtendedType = interfaceType.GetExtendedType();
            if (interfaceExtendedType.IsGeneric)
            {
                // match interface and generic arguments
                var genericArguments = interfaceType.GetGenericArguments();
                var genericInterface = Interfaces.Where(x => x.IsGenericType
                    && x.Name == interfaceExtendedType.Name
                    && x.GetGenericArguments().Length.Equals(genericArguments.Length)
                    && (genericArguments.SequenceEqual(x.GetGenericArguments())
                        || genericArguments.All(y => y.IsGenericParameter)
                    )
                ).FirstOrDefault();
                return genericInterface != null;
            }
            // match interface
            var nonGenericInterface = Interfaces.Where(x => !x.IsGenericType && x.Equals(interfaceType)).FirstOrDefault();
            return nonGenericInterface != null;
        }

        /// <summary>
        /// Returns true if the type implements an interface
        /// </summary>
        /// <typeparam name="TInterface">The interface type to check</typeparam>
        /// <returns></returns>
        public bool Implements<TInterface>() => Implements(typeof(TInterface));

        /// <summary>
        /// Returns true if type inherits from <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns></returns>
        public bool InheritsFrom(Type type)
        {
            if (BaseTypes == null)
                return false;
            return BaseTypes.Contains(type);
        }

        /// <summary>
        /// Returns true if type inherits from <seealso cref="TType"/>
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public bool InheritsFrom<TType>() => InheritsFrom(typeof(TType));

        public override bool Equals(object obj)
        {
            if (obj == null || (obj.GetType() != typeof(ExtendedType) && obj.GetType() != typeof(Type)))
                return false;
            if (obj is ExtendedType)
            {
                var objTyped = (ExtendedType)obj;
                return Equals(objTyped);
            }
            else if (obj is Type)
            {
                return Equals((Type)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Type.Name} ({UnderlyingType.Name})";
        }

        public bool Equals(ExtendedType other)
        {
            if (other is null || other.GetType() != typeof(ExtendedType))
                return false;
            return IsEqualTo(other);
        }

        public bool Equals(Type other)
        {
            if (other is null)
                return false;
            return Type.Equals(other);
        }

        public static bool operator ==(ExtendedType left, ExtendedType right)
        {
            if ((left is null && !(right is null)) || (right is null && !(left is null)))
                return false;
            return left.Equals(right);
        }

        public static bool operator !=(ExtendedType left, ExtendedType right)
        {
            return !(left == right);
        }

        public static bool operator ==(ExtendedType left, Type right)
        {
            if ((left is null && !(right is null)) || (right is null && !(left is null)))
                return false;
            return left.Equals(right);
        }

        public static bool operator !=(ExtendedType left, Type right)
        {
            return !(left == right);
        }

        public static bool operator ==(Type left, ExtendedType right)
        {
            if ((left is null && !(right is null)) || (right is null && !(left is null)))
                return false;
            return left.Equals(right);
        }

        public static bool operator !=(Type left, ExtendedType right)
        {
            return !(left == right);
        }

        public static implicit operator ExtendedType(Type type)
            => ExtendedTypeCache.GetOrCreate(type, TypeSupportOptions.All);
        
        public static implicit operator Type(ExtendedType type) => type.Type;

        private bool IsEqualTo(ExtendedType type)
        {
            var isEqual = false;
            if (_options == type._options
            && HasEmptyConstructor == type.HasEmptyConstructor
            && BaseHasEmptyConstructor == type.BaseHasEmptyConstructor
            && IsAbstract == type.IsAbstract
            && IsImmutable == type.IsImmutable
            && IsEnumerable == type.IsEnumerable
            && IsCollection == type.IsCollection
            && IsArray == type.IsArray
            && IsDictionary == type.IsDictionary
            && IsHashtable == type.IsHashtable
            && IsCollectionReadOnly == type.IsCollectionReadOnly
            && IsKeyValuePair == type.IsKeyValuePair
            && IsGeneric == type.IsGeneric
            && IsDelegate == type.IsDelegate
            && IsValueType == type.IsValueType
            && IsReferenceType == type.IsReferenceType
            && IsStruct == type.IsStruct
            && IsPrimitive == type.IsPrimitive
            && IsEnum == type.IsEnum
            && IsTuple == type.IsTuple
            && IsValueTuple == type.IsValueTuple
            && IsNullable == type.IsNullable
            && IsInterface == type.IsInterface
            && IsSerializable == type.IsSerializable
            && IsAnonymous == type.IsAnonymous
            && IsConcreteType == type.IsConcreteType
            && Enumerable.SequenceEqual(BaseTypes, type.BaseTypes)
            && Enumerable.SequenceEqual(EnumValues, type.EnumValues)
            && Enumerable.SequenceEqual(KnownConcreteTypes, type.KnownConcreteTypes)
            && Enumerable.SequenceEqual(Attributes, type.Attributes)
            && Enumerable.SequenceEqual(GenericArgumentTypes, type.GenericArgumentTypes)
            && Enumerable.SequenceEqual(Interfaces, type.Interfaces)
            && Enumerable.SequenceEqual(Constructors, type.Constructors)
            && Enumerable.SequenceEqual(EmptyConstructors, type.EmptyConstructors)
            && ConcreteType == type.ConcreteType
            && ElementType == type.ElementType
            && ElementNullableBaseType == type.ElementNullableBaseType
            && EnumType == type.EnumType
            && UnderlyingType == type.UnderlyingType
            && NullableBaseType == type.NullableBaseType

            // compare internal values so we don't trigger an enumeration
            && ((_properties == null && type._properties == null) || Enumerable.SequenceEqual(_properties, type._properties))
            && ((_fields == null && type._fields == null) || Enumerable.SequenceEqual(_fields, type._fields))
            && ((_methods == null && type._methods == null) || Enumerable.SequenceEqual(_methods, type._methods))
            && _indexerType == type._indexerType
            && _indexerReturnType == type._indexerReturnType
            && _hasIndexer == type._hasIndexer)
                isEqual = true;
            return isEqual;
        }

        /// <summary>
        /// Clone a copy of an <see cref="ExtendedType"/>
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new ExtendedType(this);
        }
    }

    /// <summary>
    /// Helper class for getting information about a <see cref="Type"/>
    /// </summary>
    public class ExtendedType<T> : ExtendedType
    {
        public ExtendedType() : base(typeof(T))
        {
        }
    }
}
