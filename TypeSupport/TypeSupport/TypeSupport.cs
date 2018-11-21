using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using TypeSupport.Extensions;

namespace TypeSupport
{
    /// <summary>
    /// Helper class for getting information about a <see cref="Type"/>
    /// </summary>
    public class TypeSupport : IEquatable<TypeSupport>, IEquatable<Type>
    {
        /// <summary>
        /// The type TypeSupport was created from
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// True if type has an empty constructor defined
        /// </summary>
        public bool HasEmptyConstructor { get; private set; }

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
        /// Trie if the type implements IDictionary
        /// </summary>
        public bool IsDictionary { get; private set; }

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
        public TypeSupport(Type type)
        {
            Type = type ?? throw new ArgumentNullException();
            InspectType();
        }

        /// <summary>
        /// For interface types, you can provide an object instance to determine a concrete type for it
        /// </summary>
        /// <param name="concreteObject"></param>
        public void SetConcreteTypeFromInstance(object concreteObject)
        {
            if(concreteObject != null)
                ConcreteType = concreteObject.GetType();
        }

        private void InspectType()
        {
            var emptyConstructorDefined = Type.GetConstructor(Type.EmptyTypes);
            Attributes = new List<Type>();
            GenericArgumentTypes = new List<Type>();
            ConcreteType = Type;
            HasEmptyConstructor = Type.IsValueType || emptyConstructorDefined != null;
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
            IsPrimitive = Type.IsPrimitive;
            IsInterface = Type.IsInterface;
            if (IsInterface)
                KnownConcreteTypes = GetConcreteTypes(Type);

            IsEnum = Type.IsEnum;
            if (IsEnum)
            {
                EnumType = Type.GetEnumUnderlyingType();
                EnumValues = Type.ToListOfKeyValuePairs(EnumType).ToList();
            }

            if (IsGeneric)
            {
                var args = Type.GetGenericArguments();
                if(args?.Any() == true)
                {
                    foreach(var arg in args)
                        GenericArgumentTypes.Add(arg);
                }
            }

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

            if (Type.IsGenericType
                && (typeof(ICollection<>).IsAssignableFrom(Type.GetGenericTypeDefinition())
                || typeof(Collection<>).IsAssignableFrom(Type.GetGenericTypeDefinition()))
            )
            {
                IsCollection = true;
                var args = Type.GetGenericArguments();
                ElementType = args.FirstOrDefault();
                if (ElementType != null)
                    ElementNullableBaseType = GetNullableBaseType(ElementType);
            }

            if (Type.IsGenericType && (Type.GetGenericTypeDefinition() == typeof(Dictionary<,>) || Type.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
                IsDictionary = true;
            if (typeof(Delegate).IsAssignableFrom(Type))
                IsDelegate = true;

            // nullable
            var nullableBaseType = GetNullableBaseType(Type);
            NullableBaseType = nullableBaseType ?? Type;
            IsNullable = nullableBaseType != null;

            // attributes
            if (Type.CustomAttributes.Any())
                Attributes = Type.CustomAttributes.Select(x => x.AttributeType).ToList();
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
            var typeAssembly = Assembly.GetAssembly(type);

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
            if(KnownConcreteTypes != null)
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
            var objTyped = (TypeSupport)obj;
            return objTyped.Type.Equals(Type);
        }

        public override string ToString()
        {
            return $"{Type.Name} ({UnderlyingType.Name})";
        }

        public bool Equals(TypeSupport other)
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
    public class TypeSupport<T> : TypeSupport
    {
        public TypeSupport() : base(typeof(T))
        {
        }
    }
}
