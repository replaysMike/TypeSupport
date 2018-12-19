﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using TypeSupport.Extensions;

namespace TypeSupport
{
    /// <summary>
    /// Factory for creating any type of object
    /// </summary>
    public class ObjectFactory
    {
        /// <summary>
        /// Get the type registry
        /// </summary>
        public TypeRegistry TypeRegistry { get; }

        /// <summary>
        /// Create a factory for creating any type of object
        /// </summary>
        public ObjectFactory()
        {

        }

        /// <summary>
        /// Create a factory for creating any type of object
        /// </summary>
        /// <param name="typeRegistry">A type registry for custom mappings</param>
        public ObjectFactory(TypeRegistry typeRegistry)
        {
            TypeRegistry = typeRegistry;
        }

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="type">The type of object to construct</param>
        /// <returns></returns>
        public object CreateEmptyObject(Type type)
        {
            return CreateEmptyObject(type, TypeRegistry, null, 0);
        }

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="type">The type of object to construct</param>
        /// <param name="typeRegistry">A type registry for constructing unknown types</param>
        /// <returns></returns>
        public object CreateEmptyObject(Type type, TypeRegistry typeRegistry)
        {
            return CreateEmptyObject(type, typeRegistry, null, 0);
        }

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="type">The type of object to construct</param>
        /// <param name="typeRegistry">A type registry for constructing unknown types</param>
        /// <param name="typeDescriptor">A type descriptor that indicates the embedded concrete type for an interface type</param>
        /// <param name="initializer">An optional initializer to use to create the object</param>
        /// <param name="length">For array types, the length of the array to create</param>
        /// <returns></returns>
        public object CreateEmptyObject(Type type, TypeRegistry typeRegistry, Func<object> initializer, int length)
        {
            if (initializer != null)
            {
                return initializer();
            }

            // check the type registry for a custom type mapping
            if (typeRegistry?.ContainsType(type) == true)
                type = typeRegistry.GetMapping(type);
            // check the type registry for a custom type factory
            if (typeRegistry?.ContainsFactoryType(type) == true)
                return typeRegistry.GetFactory(type).Invoke();

            var typeSupport = new ExtendedType(type);
            // if we are asked to create an instance of an interface, try to initialize using a valid concrete type
            if (typeSupport.IsInterface && !typeSupport.IsEnumerable)
            {
                // try a known concrete type from typeSupport
                var concreteType = typeSupport.KnownConcreteTypes.FirstOrDefault();
                if (concreteType == null)
                    throw new TypeSupportException(type, $"Unable to locate a concrete type for '{typeSupport.Type.FullName}'! Cannot create instance.");

                typeSupport = new ExtendedType(concreteType);
            }

            if (typeSupport.IsArray)
                return Activator.CreateInstance(typeSupport.Type, new object[] { length });
            else if (typeSupport.IsDictionary)
            {
                var genericType = typeSupport.Type.GetGenericArguments().ToList();
                if (genericType.Count != 2)
                    throw new TypeSupportException(type, "IDictionary should contain 2 element types.");
                Type[] typeArgs = { genericType[0], genericType[1] };

                var listType = typeof(Dictionary<,>).MakeGenericType(typeArgs);
                var newDictionary = Activator.CreateInstance(listType) as IDictionary;
                return newDictionary;
            }
            else if (typeSupport.IsEnumerable && !typeSupport.IsImmutable)
            {
                var genericType = typeSupport.Type.GetGenericArguments().FirstOrDefault();
                // test specifically for ICollection and List
                var isGenericCollection = typeSupport.Type.IsGenericType
                            && typeSupport.Type.GetGenericTypeDefinition() == typeof(ICollection<>);
                var isGenericList = typeSupport.Type.IsGenericType
                            && typeSupport.Type.GetGenericTypeDefinition() == typeof(List<>);
                var isGenericIList = typeSupport.Type.IsGenericType
                           && typeSupport.Type.GetGenericTypeDefinition() == typeof(IList<>);
                if (!isGenericList && !isGenericIList && !isGenericCollection)
                {
                    var constructors = typeSupport.Type.GetConstructors(ConstructorOptions.All);
                    if (typeSupport.HasEmptyConstructor)
                    {
                        var newList = Activator.CreateInstance(typeSupport.Type);
                        return newList;
                    }
                    else if (typeSupport.Constructors.Any())
                    {
                        // special handling is required here as custom collections can't be properly
                        // initialized using FormatterServices.GetUninitializedObject()
                        var constructor = typeSupport.Constructors.First();
                        var parameters = constructor.GetParameters();
                        var param = new List<object>();
                        // initialize using defaults for constructor parameters
                        foreach (var p in parameters)
                            param.Add(CreateEmptyObject(p.ParameterType));
                        var newList = Activator.CreateInstance(typeSupport.Type, param.ToArray());
                        return newList;
                    }
                }
                else if (genericType != null)
                {
                    var listType = typeof(List<>).MakeGenericType(genericType);
                    var newList = (IList)Activator.CreateInstance(listType);
                    return newList;
                }
                return Enumerable.Empty<object>();
            }
            else if (typeSupport.Type.ContainsGenericParameters)
            {
                // create a generic type and create an instance
                // to accomplish this, we need to create a new generic type using the type arguments from the interface
                // and the concrete class definition. voodoo!
                var originalTypeSupport = new ExtendedType(type);
                var genericArguments = originalTypeSupport.Type.GetGenericArguments();
                var newType = typeSupport.Type.MakeGenericType(genericArguments);
                object newObject = null;
                if (typeSupport.HasEmptyConstructor)
                    newObject = Activator.CreateInstance(newType);
                else
                    newObject = FormatterServices.GetUninitializedObject(newType);
                return newObject;
            }
            else if (typeSupport.HasEmptyConstructor && !typeSupport.Type.ContainsGenericParameters)
                return Activator.CreateInstance(typeSupport.Type);
            else if (typeSupport.IsImmutable)
                return null;

            return FormatterServices.GetUninitializedObject(typeSupport.Type);
        }

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="assemblyQualifiedFullName">The full name of the type to create, <see cref="Type.AssemblyQualifiedName"/></param>
        /// <returns></returns>
        public object CreateEmptyObject(string assemblyQualifiedFullName)
        {
            return CreateEmptyObject(assemblyQualifiedFullName, TypeRegistry, null, 0);
        }

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="assemblyQualifiedFullName">The full name of the type to create, <see cref="Type.AssemblyQualifiedName"/></param>
        /// <param name="initializer">An optional initializer to use to create the object</param>
        /// <param name="length">For array types, the length of the array to create</param>
        /// <returns></returns>
        public object CreateEmptyObject(string assemblyQualifiedFullName, Func<object> initializer, int length)
        {
            return CreateEmptyObject(assemblyQualifiedFullName, TypeRegistry, initializer, length);
        }

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="assemblyQualifiedFullName">The full name of the type to create, <see cref="Type.AssemblyQualifiedName"/></param>
        /// <param name="initializer">An optional initializer to use to create the object</param>
        /// <param name="length">For array types, the length of the array to create</param>
        /// <returns></returns>
        public object CreateEmptyObject(string assemblyQualifiedFullName, TypeRegistry typeRegistry, Func<object> initializer, int length)
        {
            if (string.IsNullOrEmpty(assemblyQualifiedFullName))
                throw new ArgumentNullException(nameof(assemblyQualifiedFullName));
            var type = Type.GetType(assemblyQualifiedFullName);
            return CreateEmptyObject(type, typeRegistry, initializer, length);
        }

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="type">The type to create</param>
        /// <param name="initializer">An optional initializer to use to create the object</param>
        /// <param name="length">For array types, the length of the array to create</param>
        /// <returns></returns>
        public object CreateEmptyObject(Type type, Func<object> initializer, int length)
        {
            return CreateEmptyObject(type, TypeRegistry, initializer, length);
        }

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="typeRegistry">A type registry that specifies custom mappings or factories</param>
        /// <param name="initializer">An optional initializer to use to create the object</param>
        /// <param name="length">For array types, the length of the array to create</param>
        /// <returns></returns>
        public T CreateEmptyObject<T>(Func<T> initializer, int length)
        {
            Func<object> init = null;
            if (initializer != null)
                init = () => initializer();
            return (T)CreateEmptyObject(typeof(T), init, length);
        }

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="typeRegistry">A type registry that specifies custom mappings or factories</param>
        /// <param name="initializer">An optional initializer to use to create the object</param>
        /// <param name="length">For array types, the length of the array to create</param>
        /// <returns></returns>
        public T CreateEmptyObject<T>(TypeRegistry typeRegistry, Func<T> initializer, int length)
        {
            return (T)CreateEmptyObject(typeof(T), typeRegistry, initializer as Func<object>, length);
        }
    }
}
