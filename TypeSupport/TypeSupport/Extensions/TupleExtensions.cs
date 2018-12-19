﻿using System;
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
        private static readonly HashSet<Type> ValueTupleTypes = new HashSet<Type>(new Type[]
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
        private static readonly HashSet<Type> TupleTypes = new HashSet<Type>(new []
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
            Type type = ValueTupleTypes.Skip(types.Count - 1).First();
            return type.MakeGenericType(types.ToArray());
        }

        /// <summary>
        /// Create a new Tuple
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static Type CreateTuple(ICollection<Type> types)
        {
            Type type = TupleTypes.Skip(types.Count - 1).First();
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
                && ValueTupleTypes.Contains(type.GetGenericTypeDefinition());
#else
            return type.IsGenericType
                && ValueTupleTypes.Contains(type.GetGenericTypeDefinition());
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
                && TupleTypes.Contains(type.GetGenericTypeDefinition());
#else
            return type.IsGenericType
                && TupleTypes.Contains(type.GetGenericTypeDefinition());
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
    }
}
