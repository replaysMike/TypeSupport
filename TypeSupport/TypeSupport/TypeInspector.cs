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
    /// Inspects a type
    /// </summary>
    internal class TypeInspector
    {
        internal ExtendedType _extendedType;
        internal TypeSupportOptions _options;

        /// <summary>
        /// Inspects a type
        /// </summary>
        /// <param name="extendedType">The extended type to inspect</param>
        /// <param name="options">The options to use for inspection</param>
        internal TypeInspector(ExtendedType extendedType, TypeSupportOptions options)
        {
            _extendedType = extendedType;
            _options = options;
        }

        /// <summary>
        /// Inspect the extended type and populate its properties
        /// </summary>
        internal void Inspect()
        {
            if (_options.BitwiseHasFlag(TypeSupportOptions.Properties))
                _extendedType.Properties = _extendedType.Type.GetProperties(PropertyOptions.All);
            if (_options.BitwiseHasFlag(TypeSupportOptions.Fields))
                _extendedType.Fields = _extendedType.Type.GetFields(FieldOptions.All);
            if (_options.BitwiseHasFlag(TypeSupportOptions.Methods))
                _extendedType.Methods = _extendedType.Type.GetMethods(MethodOptions.All);

            if (_options.BitwiseHasFlag(TypeSupportOptions.Attributes))
            {
#if FEATURE_CUSTOM_ATTRIBUTES
                if (_extendedType.Type.CustomAttributes.Any())
                    _extendedType.Attributes = _extendedType.Type.CustomAttributes.Select(x => x.AttributeType).ToList();
#else
            // attributes
            if (_extendedType.Type.GetCustomAttributesData().Any())
                _extendedType.Attributes = _extendedType.Type.GetCustomAttributesData().Select(x => x.Constructor.DeclaringType).ToList();
#endif
            }

            if (_options.BitwiseHasFlag(TypeSupportOptions.Constructors))
            {
                var allConstructors = _extendedType.Type.GetConstructors(ConstructorOptions.All);
                var allEmptyConstructors = allConstructors.Where(x => x.GetParameters().Any() == false).ToList();
                ConstructorInfo emptyConstructorDefined = null;
                emptyConstructorDefined = _extendedType.Type.GetConstructor(Type.EmptyTypes);
                _extendedType.HasEmptyConstructor = _extendedType.Type.IsValueType || emptyConstructorDefined != null;
                _extendedType.BaseHasEmptyConstructor = (_extendedType.Type.BaseType?.IsValueType == true || allEmptyConstructors?.Any() == true);
                _extendedType.Constructors = allConstructors;
                _extendedType.EmptyConstructors = allEmptyConstructors;
            }

            _extendedType.ConcreteType = _extendedType.Type;
            _extendedType.IsAbstract = _extendedType.Type.IsAbstract;
            _extendedType.IsSerializable = _extendedType.Type.IsSerializable;
            _extendedType.UnderlyingType = _extendedType.Type.UnderlyingSystemType;
            if (_extendedType.Type == typeof(string))
                _extendedType.IsImmutable = true;

            // collections support
            _extendedType.IsArray = _extendedType.Type.IsArray;
            _extendedType.IsGeneric = _extendedType.Type.IsGenericType;
            if (_extendedType.IsArray)
            {
                _extendedType.ElementType = GetElementType(_extendedType.Type);
                if (_extendedType.ElementType != null)
                    _extendedType.ElementNullableBaseType = GetNullableBaseType(_extendedType.ElementType);
            }
            _extendedType.IsTuple = _extendedType.Type.IsValueTupleType() || _extendedType.Type.IsTupleType();
            _extendedType.IsValueTuple = _extendedType.Type.IsValueTupleType();
            _extendedType.IsValueType = _extendedType.Type.IsValueType;
            _extendedType.IsReferenceType = _extendedType.Type.IsClass;
            _extendedType.IsStruct = _extendedType.Type.IsValueType && !_extendedType.Type.IsEnum && !_extendedType.Type.IsPrimitive && !_extendedType.Type.IsClass;
            _extendedType.IsPrimitive = _extendedType.Type.IsPrimitive;
            _extendedType.IsInterface = _extendedType.Type.IsInterface;
            if (_extendedType.IsInterface && _options.BitwiseHasFlag(TypeSupportOptions.ConcreteTypes))
                _extendedType.KnownConcreteTypes = GetConcreteTypes(_extendedType.Type);
            _extendedType.Interfaces = _extendedType.Type.GetInterfaces();
            _extendedType.IsEnum = _extendedType.Type.IsEnum;
            if (_extendedType.IsEnum)
            {
                _extendedType.EnumType = _extendedType.Type.GetEnumUnderlyingType();
                _extendedType.EnumValues = _extendedType.Type.ToListOfKeyValuePairs(_extendedType.EnumType).ToList();
            }

            if (_options.BitwiseHasFlag(TypeSupportOptions.Collections))
            {
                if (typeof(IEnumerable).IsAssignableFrom(_extendedType.Type))
                {
                    _extendedType.IsEnumerable = true;
                    if (_extendedType.IsGeneric)
                    {
                        var genericArgument = _extendedType.Type.GetGenericArguments().FirstOrDefault();
                        if (genericArgument != null)
                            _extendedType.ElementType = genericArgument;
                        if (_extendedType.ElementType != null)
                            _extendedType.ElementNullableBaseType = GetNullableBaseType(_extendedType.ElementType);
                    }

                    // inspect the interfaces for a generic type
                    if (_extendedType.ElementType == null)
                    {
                        foreach (var @interface in _extendedType.Interfaces)
                        {
                            // if the class implements any generic interface, it can be considered generic
                            if (@interface.IsAssignableFrom(_extendedType.Type) && @interface.IsGenericType)
                            {
                                _extendedType.ElementType = GetElementTypeForInterfaceType(@interface);
                            }
                        }
                    }
                }
            }

            if (_extendedType.IsGeneric)
            {
                InspectGenericType(_extendedType, _extendedType.Type, _options);
            }
            else
            {
                if (_options.BitwiseHasFlag(TypeSupportOptions.Collections))
                {
                    if (!_extendedType.Type.IsArray
                        && (typeof(ICollection).IsAssignableFrom(_extendedType.Type)
                            || typeof(IList).IsAssignableFrom(_extendedType.Type)
                            )
                        )
                    {
                        _extendedType.IsCollection = true;
                        _extendedType.ElementType = typeof(object);
                        _extendedType.GenericArgumentTypes.Add(typeof(object));
                    }

                    if (typeof(IDictionary).IsAssignableFrom(_extendedType.Type))
                    {
                        _extendedType.IsDictionary = true;
                        _extendedType.ElementType = typeof(object);
                    }
                }
            }

            if (typeof(Delegate).IsAssignableFrom(_extendedType.Type))
                _extendedType.IsDelegate = true;

            if (_options.BitwiseHasFlag(TypeSupportOptions.Indexers))
            {
                _extendedType.HasIndexer = _extendedType.Properties.Select(x => x.PropertyInfo.GetIndexParameters())
                .Where(x => x.Length > 0)
                .Any();

                if (_extendedType.HasIndexer)
                {
                    // c# only allows a single indexer
                    var indexerProperty = _extendedType.Properties.Where(x => x.PropertyInfo.GetIndexParameters().Length > 0).ToList();
                    var indexParameters = indexerProperty.FirstOrDefault().PropertyInfo.GetIndexParameters().FirstOrDefault();
                    _extendedType.IndexerType = indexParameters.ParameterType;
                    _extendedType.IndexerReturnType = indexerProperty.FirstOrDefault().PropertyInfo.PropertyType;
                }
            }

            // nullable
            var nullableBaseType = GetNullableBaseType(_extendedType.Type);
            _extendedType.NullableBaseType = nullableBaseType ?? _extendedType.Type;
            _extendedType.IsNullable = nullableBaseType != null;

            // anonymous
            _extendedType.IsAnonymous = Attribute.IsDefined(_extendedType.Type, typeof(CompilerGeneratedAttribute), false)
                && _extendedType.Type.IsGenericType && _extendedType.Type.Name.Contains("AnonymousType")
                && (_extendedType.Type.Name.StartsWith("<>") || _extendedType.Type.Name.StartsWith("VB$"))
                && (_extendedType.Type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;

            // provide a way to detect if the type requires additional concrete information in order to be serialized
            _extendedType.IsConcreteType = !(_extendedType.IsAbstract || _extendedType.IsInterface || _extendedType.IsAnonymous || _extendedType.Type == typeof(object));
        }

        private Type GetElementTypeForInterfaceType(Type type)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            var args = type.GetGenericArguments();
            var genericArgumentTypes = new List<Type>();
            if (args?.Any() == true)
            {
                foreach (var arg in args)
                {
                    if (!genericArgumentTypes.Contains(arg))
                        genericArgumentTypes.Add(arg);
                }
            }
            var elementType = genericArgumentTypes.FirstOrDefault();

            return elementType;
        }

        private void InspectGenericType(ExtendedType extendedType, Type type, TypeSupportOptions options)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            var args = type.GetGenericArguments();
            if (args?.Any() == true)
            {
                foreach (var arg in args)
                {
                    extendedType.GenericArgumentTypes.Add(arg);
                }
            }

            if (options.BitwiseHasFlag(TypeSupportOptions.Collections))
            {
                if (typeof(ICollection).IsAssignableFrom(genericTypeDefinition)
                    || typeof(IList) == type
                    || typeof(IList).IsAssignableFrom(genericTypeDefinition)
                    || typeof(IList<>).IsAssignableFrom(genericTypeDefinition)
                    || typeof(ICollection<>).IsAssignableFrom(genericTypeDefinition)
                    || typeof(Collection<>).IsAssignableFrom(genericTypeDefinition)
                    )
                {
                    extendedType.IsCollection = true;
                    extendedType.ElementType = args.FirstOrDefault();
                    if (extendedType.ElementType != null)
                        extendedType.ElementNullableBaseType = GetNullableBaseType(extendedType.ElementType);
                }

                if (genericTypeDefinition == typeof(Dictionary<,>)
                    || genericTypeDefinition == typeof(ConcurrentDictionary<,>)
                    || genericTypeDefinition == typeof(IDictionary<,>))
                    extendedType.IsDictionary = true;
            }
            if (genericTypeDefinition == typeof(KeyValuePair<,>))
            {
                extendedType.IsKeyValuePair = true;
                extendedType.ElementType = args.FirstOrDefault();
            }
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

            var baseType = givenType.BaseType;
            if (baseType == null)
                return false;

            return IsAssignableToGenericType(baseType, genericType);
        }

        /// <summary>
        /// Get the concrete type of an object instance
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Type GetConcreteType(ExtendedType extendedType, object obj)
        {
            var objectType = obj.GetType();
            if (extendedType.KnownConcreteTypes != null)
                return extendedType.KnownConcreteTypes.Where(x => objectType.IsAssignableFrom(x)).FirstOrDefault();
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
    }
}
