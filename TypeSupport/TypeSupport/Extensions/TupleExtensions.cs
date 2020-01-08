using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TypeSupport.Extensions
{
    /// <summary>
    /// Type extensions for Tuples and ValueTuples
    /// </summary>
    public static class TupleExtensions
    {
        /// <summary>
        /// Mapping for types that are generic ValueTuples
        /// </summary>
        private static readonly HashSet<Type> _valueTupleTypes = new HashSet<Type>(new Type[]
        {
#if FEATURE_CUSTOM_VALUETUPLE
            typeof(ValueTuple<>),
            typeof(ValueTuple<,>),
            typeof(ValueTuple<,,>),
            typeof(ValueTuple<,,,>),
            typeof(ValueTuple<,,,,>),
            typeof(ValueTuple<,,,,,>),
            typeof(ValueTuple<,,,,,,>),
            typeof(ValueTuple<,,,,,,,>)
#endif
        });

        /// <summary>
        /// Mapping for types that are generic Tuples
        /// </summary>
        private static readonly HashSet<Type> _tupleTypes = new HashSet<Type>(new []
        {
            typeof(Tuple<>),
            typeof(Tuple<,>),
            typeof(Tuple<,,>),
            typeof(Tuple<,,,>),
            typeof(Tuple<,,,,>),
            typeof(Tuple<,,,,,>),
            typeof(Tuple<,,,,,,>),
            typeof(Tuple<,,,,,,,>)
        });

        /// <summary>
        /// Create a new ValueTuple
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static Type CreateValueTuple(ICollection<Type> types)
        {
            Type type = _valueTupleTypes.Skip(types.Count - 1).First();
            return type.MakeGenericType(types.ToArray());
        }

        /// <summary>
        /// Create a new Tuple
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static Type CreateTuple(ICollection<Type> types)
        {
            Type type = _tupleTypes.Skip(types.Count - 1).First();
            return type.MakeGenericType(types.ToArray());
        }

        /// <summary>
        /// True if an object is a ValueTuple
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsValueTuple(this object obj) => IsValueTupleType(obj.GetType());

        /// <summary>
        /// True if an object is a Tuple
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsTuple(this object obj) => IsTupleType(obj.GetType());

        /// <summary>
        /// True if the type is a ValueTuple
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsValueTupleType(this Type type)
        {
#if FEATURE_CUSTOM_TYPEINFO
            return type.GetTypeInfo().IsGenericType
                && _valueTupleTypes.Contains(type.GetGenericTypeDefinition());
#else
            return type.IsGenericType
                && _valueTupleTypes.Contains(type.GetGenericTypeDefinition());
#endif
        }

        /// <summary>
        /// True if the type is a Tuple
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsTupleType(this Type type)
        {
#if FEATURE_CUSTOM_TYPEINFO
            return type.GetTypeInfo().IsGenericType
                && _tupleTypes.Contains(type.GetGenericTypeDefinition());
#else
            return type.IsGenericType
                && _tupleTypes.Contains(type.GetGenericTypeDefinition());
#endif
        }

        /// <summary>
        /// Get the items within a Tuple
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static List<object> GetValueTupleItemObjects(this object tuple)
            => GetValueTupleItemFields(tuple.GetType())
                .Select(f => f.GetValue(tuple))
            .ToList();

        /// <summary>
        /// Get the items within a Tuple
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static List<object> GetTupleItemObjects(this object tuple)
            => GetTupleItemFields(tuple.GetType())
                .Select(f => f.GetValue(tuple))
            .ToList();

        /// <summary>
        /// Get the type values of a ValueTuple
        /// </summary>
        /// <param name="tupleType"></param>
        /// <returns></returns>
        public static List<Type> GetValueTupleItemTypes(this Type tupleType)
            => GetValueTupleItemFields(tupleType)
            .Select(f => f.FieldType)
            .ToList();

        /// <summary>
        /// Get the type values of a Tuple
        /// </summary>
        /// <param name="tupleType"></param>
        /// <returns></returns>
        public static List<Type> GetTupleItemTypes(this Type tupleType)
            => GetTupleItemFields(tupleType)
            .Select(f => f.FieldType)
            .ToList();

        /// <summary>
        /// Get a particular Item field from a ValueTuple
        /// </summary>
        /// <param name="tupleType"></param>
        /// <returns></returns>
        public static List<FieldInfo> GetValueTupleItemFields(this Type tupleType)
        {
            var items = new List<FieldInfo>();

            FieldInfo field;
            int nth = 1;
            while ((field = tupleType.GetField($"Item{nth}", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) != null)
            {
                nth++;
                items.Add(field);
            }

            return items;
        }

        /// <summary>
        /// Get a particular Item field from a Tuple
        /// </summary>
        /// <param name="tupleType"></param>
        /// <returns></returns>
        public static List<FieldInfo> GetTupleItemFields(this Type tupleType)
        {
            var items = new List<FieldInfo>();

            FieldInfo field;
            int nth = 1;
            while ((field = tupleType.GetField($"m_Item{nth}", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) != null)
            {
                nth++;
                items.Add(field);
            }

            return items;
        }
#if FEATURE_CUSTOM_VALUETUPLE
        /// <summary>
        /// Get the names of a named value tuple contained in a method or constructor
        /// </summary>
        /// <param name="tuple"></param>
        /// <param name="container">The container that declared the named tuple (method or constructor)</param>
        /// <param name="tupleType">The optional type of the named tuple, if inspecting a constructor</param>
        /// <returns></returns>
        /// <example>
        ///  From tuple declared in a constructor:
        ///     var instance = new ObjectWithNamedTuple((id: 1, value: "test"));
        ///     var tupleType = typeof((int, string));
        ///     var names = instance.GetTupleNamedParameters(instance.GetType().GetConstructor(new Type[] { tupleType }), tupleType);
        ///  From tuple declared in a method:
        ///     private ICollection<string> InspectNamedValueTuple((int id, string value) tuple) {
        ///         return tuple.GetTupleNamedParameters(System.Reflection.MethodBase.GetCurrentMethod());
        ///     }
        /// </example>
        public static ICollection<string> GetTupleNamedParameters(this object tuple, MethodBase container, Type tupleType = null)
        {
            if (tupleType == null)
                tupleType = tuple.GetType();
            var parameters = container.GetParameters().FirstOrDefault(x => x.ParameterType.Name.Equals(tupleType.Name));
            var elementAttributes = parameters?.GetCustomAttributes().FirstOrDefault(t => t.GetType() == typeof(System.Runtime.CompilerServices.TupleElementNamesAttribute)) as System.Runtime.CompilerServices.TupleElementNamesAttribute;
            return elementAttributes?.TransformNames ?? new List<string>();
        }

        /// <summary>
        /// Get the names of a named value tuple contained in a class
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="fieldName">The property or field name that stores the tuple</param>
        /// <returns></returns>
        /// <example>
        ///     var instance = new ObjectWithNamedTuple();
        ///     var names = instance.GetTupleNamedParameters(nameof(instance.TupleValue));
        /// </example>
        public static ICollection<string> GetTupleNamedParameters(this object instance, string fieldName)
        {
            var property = instance.GetProperty(fieldName);               
            var elementAttributes = property.GetCustomAttributes().FirstOrDefault(t => t.GetType() == typeof(System.Runtime.CompilerServices.TupleElementNamesAttribute)) as System.Runtime.CompilerServices.TupleElementNamesAttribute;
            if (elementAttributes == null)
            {
                var field = instance.GetField(fieldName);
                elementAttributes = field.GetCustomAttributes().FirstOrDefault(t => t.GetType() == typeof(System.Runtime.CompilerServices.TupleElementNamesAttribute)) as System.Runtime.CompilerServices.TupleElementNamesAttribute;
            }
            return elementAttributes?.TransformNames ?? new List<string>();
        }
#endif
    }
}
