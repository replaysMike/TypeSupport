using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
        public bool HasEmptyConstructor { get; private set; }

        /// <summary>
        /// True if a base type has an empty constructor defined
        /// </summary>
        public bool BaseHasEmptyConstructor { get; private set; }

        /// <summary>
        /// True if the type is abstract
        /// </summary>
        public bool IsAbstract { get; private set; }

        /// <summary>
        /// True if type is an immutable type
        /// </summary>
        public bool IsImmutable { get; private set; }

        /// <summary>
        /// True if the type implements IEnumerable
        /// </summary>
        public bool IsEnumerable { get; private set; }

        /// <summary>
        /// True if the type implements ICollection
        /// </summary>
        public bool IsCollection { get; private set; }

        /// <summary>
        /// True if the type is an array
        /// </summary>
        public bool IsArray { get; private set; }

        /// <summary>
        /// True if the type implements IDictionary
        /// </summary>
        public bool IsDictionary { get; private set; }

        /// <summary>
        /// True if the type is a key value pair
        /// </summary>
        public bool IsKeyValuePair { get; private set; }

        /// <summary>
        /// True if the type is a generic type
        /// </summary>
        public bool IsGeneric { get; private set; }

        /// <summary>
        /// True if the type is a delegate
        /// </summary>
        public bool IsDelegate { get; private set; }

        /// <summary>
        /// True if the type is an integral value type
        /// </summary>
        public bool IsValueType { get; private set; }

        /// <summary>
        /// True if the type is a reference type
        /// </summary>
        public bool IsReferenceType { get; private set; }

        /// <summary>
        /// True if the type is a struct
        /// </summary>
        public bool IsStruct { get; private set; }

        /// <summary>
        /// True if the type is a primitive type
        /// </summary>
        public bool IsPrimitive { get; private set; }

        /// <summary>
        /// True if the type is a system enum
        /// </summary>
        public bool IsEnum { get; private set; }

        /// <summary>
        /// True if the type is a Tuple of any length <see cref="Tuple"/>
        /// </summary>
        public bool IsTuple { get; private set; }

        /// <summary>
        /// True if they type is a Tuple of any length <see cref="ValueTuple"/>
        /// </summary>
        public bool IsValueTuple { get; private set; }

        /// <summary>
        /// True if the type is nullable
        /// </summary>
        public bool IsNullable { get; private set; }

        /// <summary>
        /// True if the type is an interface
        /// </summary>
        public bool IsInterface { get; private set; }

        /// <summary>
        /// True if the type contains an indexer
        /// </summary>
        public bool HasIndexer { get; private set; }

        /// <summary>
        /// True if the type is an anonymous type
        /// </summary>
        public bool IsAnonymous { get; private set; }

        /// <summary>
        /// For enum types the list of valid values of the Enum
        /// </summary>
        public ICollection<KeyValuePair<object, string>> EnumValues { get; private set; }

        /// <summary>
        /// For interface types the list of known concrete types that implement it
        /// </summary>
        public ICollection<Type> KnownConcreteTypes { get; private set; }

        /// <summary>
        /// The list of attributes defined on the type (concrete types only)
        /// </summary>
        public ICollection<Type> Attributes { get; private set; }

        /// <summary>
        /// The list of type arguments of a generic type
        /// </summary>
        public ICollection<Type> GenericArgumentTypes { get; private set; }

        /// <summary>
        /// A list of the type's properties
        /// </summary>
        public ICollection<PropertyInfo> Properties { get; private set; }

        /// <summary>
        /// A list of the type's fields
        /// </summary>
        public ICollection<FieldInfo> Fields { get; private set; }

        /// <summary>
        /// List of implemented interfaces
        /// </summary>
        public ICollection<Type> Interfaces { get; private set; }

        /// <summary>
        /// All constructors
        /// </summary>
        public ICollection<ConstructorInfo> Constructors { get; private set; }

        /// <summary>
        /// All empty constructors
        /// </summary>
        public ICollection<ConstructorInfo> EmptyConstructors { get; private set; }

        /// <summary>
        /// The type TypeSupport was created from
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// For interface types the concrete type that implements it, <seealso cref="SetConcreteTypeFromInstance(object)"/>
        /// </summary>
        public Type ConcreteType { get; private set; }

        /// <summary>
        /// For array and enumerable types, the element data type
        /// </summary>
        public Type ElementType { get; private set; }

        /// <summary>
        /// For array and enumerable types that have a nullable value, the type of the nullable
        /// </summary>
        public Type ElementNullableBaseType { get; private set; }

        /// <summary>
        /// The declared value type of an enum (one of the following numeric types: sbyte,byte,ushort,short,uint,int,ulong,long)
        /// </summary>
        public Type EnumType { get; private set; }

        /// <summary>
        /// The declared type of an indexer key
        /// </summary>
        public Type IndexerType { get; private set; }

        /// <summary>
        /// The declared return type of an indexer key
        /// </summary>
        public Type IndexerReturnType { get; private set; }

        /// <summary>
        /// The underlying type of the Type
        /// </summary>
        public Type UnderlyingType { get; private set; }

        /// <summary>
        /// The base type of a nullable Type
        /// </summary>
        public Type NullableBaseType { get; private set; }

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
            Properties = new List<PropertyInfo>();
            Fields = new List<FieldInfo>();
            Constructors = new List<ConstructorInfo>();
            EmptyConstructors = new List<ConstructorInfo>();

            InspectType(options);
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

        private void InspectType(TypeSupportOptions options)
        {
            if (options.BitwiseHasFlag(TypeSupportOptions.Properties))
                Properties = Type.GetProperties();
            if (options.BitwiseHasFlag(TypeSupportOptions.Fields))
                Fields = Type.GetFields();

            if (options.BitwiseHasFlag(TypeSupportOptions.Attributes))
            {
#if FEATURE_CUSTOM_ATTRIBUTES
                if (Type.CustomAttributes.Any())
                    Attributes = Type.CustomAttributes.Select(x => x.AttributeType).ToList();
#else
            // attributes
            if (Type.GetCustomAttributesData().Any())
                Attributes = Type.GetCustomAttributesData().Select(x => x.Constructor.DeclaringType).ToList();
#endif
            }

            if (options.BitwiseHasFlag(TypeSupportOptions.Constructors))
            {
                var allConstructors = Type.GetConstructors(ConstructorOptions.All);
                var allEmptyConstructors = allConstructors.Where(x => x.GetParameters().Any() == true).ToList();
                var emptyConstructorDefined = Type.GetConstructor(Type.EmptyTypes);
                HasEmptyConstructor = Type.IsValueType || emptyConstructorDefined != null;
                BaseHasEmptyConstructor = (Type.BaseType?.IsValueType == true || allEmptyConstructors?.Any() == true);
                Constructors = allConstructors;
                EmptyConstructors = allEmptyConstructors;
            }

            ConcreteType = Type;
            IsAbstract = Type.IsAbstract;
            UnderlyingType = Type.UnderlyingSystemType;
            if (Type == typeof(string))
                IsImmutable = true;

            // collections support
            IsArray = Type.IsArray;
            IsGeneric = Type.IsGenericType;
            if (IsArray)
            {
                ElementType = GetElementType(Type);
                if (ElementType != null)
                    ElementNullableBaseType = GetNullableBaseType(ElementType);
            }
            IsTuple = Type.IsValueTupleType() || Type.IsTupleType();
            IsValueTuple = Type.IsValueTupleType();
            IsValueType = Type.IsValueType;
            IsReferenceType = Type.IsClass;
            IsStruct = Type.IsValueType && !Type.IsEnum && !Type.IsPrimitive && !Type.IsClass;
            IsPrimitive = Type.IsPrimitive;
            IsInterface = Type.IsInterface;
            if (IsInterface && options.BitwiseHasFlag(TypeSupportOptions.ConcreteTypes))
                KnownConcreteTypes = GetConcreteTypes(Type);
            Interfaces = Type.GetInterfaces();
            IsEnum = Type.IsEnum;
            if (IsEnum)
            {
                EnumType = Type.GetEnumUnderlyingType();
                EnumValues = Type.ToListOfKeyValuePairs(EnumType).ToList();
            }

            if (options.BitwiseHasFlag(TypeSupportOptions.Collections))
            {
                if (typeof(IEnumerable).IsAssignableFrom(Type))
                {
                    IsEnumerable = true;
                    if (IsGeneric)
                    {
                        var args = Type.GetGenericArguments();
                        ElementType = args.FirstOrDefault();
                        if (ElementType != null)
                            ElementNullableBaseType = GetNullableBaseType(ElementType);
                    }
                }
            }

            if (IsGeneric)
            {
                var genericTypeDefinition = Type.GetGenericTypeDefinition();
                var args = Type.GetGenericArguments();
                if (args?.Any() == true)
                {
                    foreach (var arg in args)
                        GenericArgumentTypes.Add(arg);
                }

                if (options.BitwiseHasFlag(TypeSupportOptions.Collections))
                {
                    if (typeof(ICollection).IsAssignableFrom(genericTypeDefinition)
                        || typeof(IList) == Type
                        || typeof(IList).IsAssignableFrom(genericTypeDefinition)
                        || typeof(IList<>).IsAssignableFrom(genericTypeDefinition)
                        || typeof(ICollection<>).IsAssignableFrom(genericTypeDefinition)
                        || typeof(Collection<>).IsAssignableFrom(genericTypeDefinition)
                        )
                    {
                        IsCollection = true;
                        ElementType = args.FirstOrDefault();
                        if (ElementType != null)
                            ElementNullableBaseType = GetNullableBaseType(ElementType);
                    }

                    if (genericTypeDefinition == typeof(Dictionary<,>)
                        || genericTypeDefinition == typeof(ConcurrentDictionary<,>)
                        || genericTypeDefinition == typeof(IDictionary<,>))
                        IsDictionary = true;
                }
                if (genericTypeDefinition == typeof(KeyValuePair<,>))
                {
                    IsKeyValuePair = true;
                    ElementType = args.FirstOrDefault();
                }
            }
            else
            {
                if (options.BitwiseHasFlag(TypeSupportOptions.Collections))
                {
                    if (!Type.IsArray
                        && (typeof(ICollection).IsAssignableFrom(Type)
                            || typeof(IList).IsAssignableFrom(Type)
                            || typeof(IList).IsAssignableFrom(Type)
                            )
                        )
                    {
                        IsCollection = true;
                        ElementType = typeof(object);
                        GenericArgumentTypes.Add(typeof(object));
                    }

                    if (typeof(IDictionary).IsAssignableFrom(Type))
                    {
                        IsDictionary = true;
                        ElementType = typeof(object);
                    }
                }
            }

            if (typeof(Delegate).IsAssignableFrom(Type))
                IsDelegate = true;

            if (options.BitwiseHasFlag(TypeSupportOptions.Indexers))
            {
                HasIndexer = Properties.Select(x => x.GetIndexParameters())
                .Where(x => x.Length > 0)
                .Any();

                if (HasIndexer)
                {
                    // c# only allows a single indexer
                    var indexerProperty = Properties.Where(x => x.GetIndexParameters().Length > 0).ToList();
                    var indexParameters = indexerProperty.FirstOrDefault().GetIndexParameters().FirstOrDefault();
                    IndexerType = indexParameters.ParameterType;
                    IndexerReturnType = indexerProperty.FirstOrDefault().PropertyType;
                }
            }

            // nullable
            var nullableBaseType = GetNullableBaseType(Type);
            NullableBaseType = nullableBaseType ?? Type;
            IsNullable = nullableBaseType != null;

            // anonymous
            IsAnonymous = Attribute.IsDefined(Type, typeof(CompilerGeneratedAttribute), false)
                && Type.IsGenericType && Type.Name.Contains("AnonymousType")
                && (Type.Name.StartsWith("<>") || Type.Name.StartsWith("VB$"))
                && (Type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        /// <summary>
        /// Get the list of concrete types that implement an interface
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ICollection<Type> GetConcreteTypes(Type type)
        {
            if (!type.IsInterface)
                throw new TypeSupportException(type, $"The type {type.Name} is not an interface. Only interface types can be analyzed.");
            var typeAssembly = System.Reflection.Assembly.GetAssembly(type);

            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                return typeAssembly.GetTypes()
                    .Select(p => new KeyValuePair<Type, Type[]>(p, p.GetInterfaces()))
                    .Where(x => x.Value.Any(y => y.IsGenericType && y.GetGenericTypeDefinition().Equals(genericType)))
                    .Select(x => x.Key)
                    .ToList();
            }
            else
            {
                var allTypes = typeAssembly.GetTypes()
                    .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                    .ToList();

                return allTypes;
            }
        }

        /// <summary>
        /// Returns true if a type can be assigned to a specific generic type
        /// </summary>
        /// <param name="givenType"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        public bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }

        /// <summary>
        /// Get the concrete type of an object instance
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Type GetConcreteType(object obj)
        {
            var objectType = obj.GetType();
            if (KnownConcreteTypes != null)
                return KnownConcreteTypes.Where(x => objectType.IsAssignableFrom(x)).FirstOrDefault();
            return objectType;
        }

        /// <summary>
        /// Get the base type of a nullable object
        /// </summary>
        /// <param name="type">Nullable type to analyse</param>
        /// <returns>Null if the type is not nullable</returns>
        public Type GetNullableBaseType(Type type)
        {
            return Nullable.GetUnderlyingType(type);
        }

        /// <summary>
        /// Get the type of an array element
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Type GetElementType(Type type)
        {
            if (!type.IsArray)
                throw new TypeSupportException(type, $"The type {type.Name} is not an array.");
            return type.GetElementType();
        }

        public override bool Equals(object obj)
        {
            var objTyped = (ExtendedType)obj;
            return objTyped.Type.Equals(Type);
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
            return other.Type.Equals(Type);
        }

        public bool Equals(Type other)
        {
            return other.Equals(Type);
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
