using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TypeSupport.Extensions;

namespace TypeSupport
{
    /// <summary>
    /// Helper class for getting information about a <see cref="Type"/>
    /// </summary>
    public class ExtendedType : IEquatable<ExtendedType>, IEquatable<Type>
    {
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
        public bool HasIndexer { get; internal set; }

        /// <summary>
        /// True if the type is an anonymous type
        /// </summary>
        public bool IsAnonymous { get; internal set; }

        /// <summary>
        /// True if the type contains an implementation or is of a concrete type (not: abstract, interfaces, anonymous types, object)
        /// </summary>
        public bool IsConcreteType { get; internal set; }

        /// <summary>
        /// For enum types the list of valid values of the Enum
        /// </summary>
        public ICollection<KeyValuePair<object, string>> EnumValues { get; internal set; }

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
        public ICollection<ExtendedProperty> Properties { get; internal set; }

        /// <summary>
        /// A list of the type's fields
        /// </summary>
        public ICollection<ExtendedField> Fields { get; internal set; }

        /// <summary>
        /// A list of the type's methods
        /// </summary>
        public ICollection<ExtendedMethod> Methods { get; internal set; }

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
        public Type IndexerType { get; internal set; }

        /// <summary>
        /// The declared return type of an indexer key
        /// </summary>
        public Type IndexerReturnType { get; internal set; }

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
        public ExtendedType(Type type) : this(type, TypeSupportOptions.All)
        {
        }

        /// <summary>
        /// Create a new type support
        /// </summary>
        /// <param name="type">The type to analyze</param>
        /// <param name="options">The type support inspection options</param>
        public ExtendedType(Type type, TypeSupportOptions options)
        {
            Type = type ?? throw new ArgumentNullException();
            Attributes = new List<Type>();
            Interfaces = new List<Type>();
            GenericArgumentTypes = new List<Type>();
            KnownConcreteTypes = new List<Type>();
            EnumValues = new List<KeyValuePair<object, string>>();
            Properties = new List<ExtendedProperty>();
            Fields = new List<ExtendedField>();
            Constructors = new List<ConstructorInfo>();
            EmptyConstructors = new List<ConstructorInfo>();

            var isCachingSupported = options.BitwiseHasFlag(TypeSupportOptions.Caching);
            // if the type is cached, use it
            if (isCachingSupported && ExtendedTypeCache.Contains(type, options))
                InitializeFromCache(ExtendedTypeCache.Get(type, options));
            else
            {
                // inspect the type with the given options
                var typeInspector = new TypeInspector(this, options);
                typeInspector.Inspect();
                if (isCachingSupported)
                    ExtendedTypeCache.CacheType(this, options);
            }
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
        /// For interface types, you can provide an object instance to determine a concrete type for it
        /// </summary>
        /// <param name="concreteObject"></param>
        public void SetConcreteTypeFromInstance(object concreteObject)
        {
            if (concreteObject != null)
                ConcreteType = concreteObject.GetType();
        }

        /// <summary>
        /// Initialize an extended type from another extended type
        /// </summary>
        /// <param name="type"></param>
        private void InitializeFromCache(ExtendedType type)
        {
            HasEmptyConstructor = type.HasEmptyConstructor;
            BaseHasEmptyConstructor = type.BaseHasEmptyConstructor;
            IsAbstract = type.IsAbstract;
            IsImmutable = type.IsImmutable;
            IsEnumerable = type.IsEnumerable;
            IsCollection = type.IsCollection;
            IsArray = type.IsArray;
            IsDictionary = type.IsDictionary;
            IsKeyValuePair = type.IsKeyValuePair;
            IsGeneric = type.IsGeneric;
            IsDelegate = type.IsDelegate;
            IsValueType = type.IsValueType;
            IsReferenceType = type.IsReferenceType;
            IsStruct = type.IsStruct;
            IsPrimitive = type.IsPrimitive;
            IsEnum = type.IsEnum;
            IsTuple = type.IsTuple;
            IsValueTuple = type.IsValueTuple;
            IsNullable = type.IsNullable;
            IsInterface = type.IsInterface;
            IsSerializable = type.IsSerializable;
            HasIndexer = type.HasIndexer;
            IsAnonymous = type.IsAnonymous;
            IsConcreteType = type.IsConcreteType;
            EnumValues = type.EnumValues;
            KnownConcreteTypes = type.KnownConcreteTypes;
            Attributes = type.Attributes;
            GenericArgumentTypes = type.GenericArgumentTypes;
            Properties = type.Properties;
            Fields = type.Fields;
            Methods = type.Methods;
            Interfaces = type.Interfaces;
            Constructors = type.Constructors;
            EmptyConstructors = type.EmptyConstructors;
            ConcreteType = type.ConcreteType;
            ElementType = type.ElementType;
            ElementNullableBaseType = type.ElementNullableBaseType;
            EnumType = type.EnumType;
            IndexerType = type.IndexerType;
            IndexerReturnType = type.IndexerReturnType;
            UnderlyingType = type.UnderlyingType;
            NullableBaseType = type.NullableBaseType;
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
                    && ( genericArguments.SequenceEqual(x.GetGenericArguments())
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
        public bool Implements<TInterface>()
        {
            return Implements(typeof(TInterface));
        }

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
            if (other == null || other.GetType() != typeof(ExtendedType))
                return false;
            return IsEqualTo(other);
        }

        public bool Equals(Type other)
        {
            if (other == null)
                return false;
            return Type.Equals(other);
        }

        private bool IsEqualTo(ExtendedType type)
        {
            var isEqual = false;
            if (HasEmptyConstructor == type.HasEmptyConstructor
            && BaseHasEmptyConstructor == type.BaseHasEmptyConstructor
            && IsAbstract == type.IsAbstract
            && IsImmutable == type.IsImmutable
            && IsEnumerable == type.IsEnumerable
            && IsCollection == type.IsCollection
            && IsArray == type.IsArray
            && IsDictionary == type.IsDictionary
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
            && HasIndexer == type.HasIndexer
            && IsAnonymous == type.IsAnonymous
            && IsConcreteType == type.IsConcreteType
            && Enumerable.SequenceEqual(EnumValues, type.EnumValues)
            && Enumerable.SequenceEqual(KnownConcreteTypes, type.KnownConcreteTypes)
            && Enumerable.SequenceEqual(Attributes, type.Attributes)
            && Enumerable.SequenceEqual(GenericArgumentTypes, type.GenericArgumentTypes)
            && Enumerable.SequenceEqual(Properties, type.Properties)
            && Enumerable.SequenceEqual(Fields, type.Fields)
            && Enumerable.SequenceEqual(Methods, type.Methods)
            && Enumerable.SequenceEqual(Interfaces, type.Interfaces)
            && Enumerable.SequenceEqual(Constructors, type.Constructors)
            && Enumerable.SequenceEqual(EmptyConstructors, type.EmptyConstructors)
            && ConcreteType == type.ConcreteType
            && ElementType == type.ElementType
            && ElementNullableBaseType == type.ElementNullableBaseType
            && EnumType == type.EnumType
            && IndexerType == type.IndexerType
            && IndexerReturnType == type.IndexerReturnType
            && UnderlyingType == type.UnderlyingType
            && NullableBaseType == type.NullableBaseType)
                isEqual = true;
            return isEqual;
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
