using System;
using System.Collections;
using System.Collections.Generic;
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
        public object CreateEmptyObject(Type type) => CreateEmptyObject(type, TypeRegistry);

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="type">The type of object to construct</param>
        /// <returns></returns>
        public object CreateEmptyObject(ExtendedType type) => CreateEmptyObject(type, TypeRegistry);

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="assemblyQualifiedFullName">The full name of the type to create, <see cref="Type.AssemblyQualifiedName"/></param>
        /// <returns></returns>
        public object CreateEmptyObject(string assemblyQualifiedFullName)
            => CreateEmptyObject(assemblyQualifiedFullName, TypeRegistry, null, 0);

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="assemblyQualifiedFullName">The full name of the type to create, <see cref="Type.AssemblyQualifiedName"/></param>
        /// <param name="typeRegistry">A type registry that specifies custom mappings or factories</param>
        /// <returns></returns>
        public object CreateEmptyObject(string assemblyQualifiedFullName, TypeRegistry typeRegistry)
            => CreateEmptyObject(assemblyQualifiedFullName, typeRegistry, null, 0);

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="assemblyQualifiedFullName">The full name of the type to create, <see cref="Type.AssemblyQualifiedName"/></param>
        /// <param name="typeRegistry">A type registry that specifies custom mappings or factories</param>
        /// <param name="dimensions">For array types, the dimensions of the array to create</param>
        /// <returns></returns>
        public object CreateEmptyObject(string assemblyQualifiedFullName, TypeRegistry typeRegistry, params object[] dimensions)
            => CreateEmptyObject(assemblyQualifiedFullName, typeRegistry, null, dimensions);

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="assemblyQualifiedFullName">The full name of the type to create, <see cref="Type.AssemblyQualifiedName"/></param>
        /// <param name="initializer">An optional initializer to use to create the object</param>
        /// <param name="dimensions">For array types, the dimensions of the array to create</param>
        /// <returns></returns>
        public object CreateEmptyObject(string assemblyQualifiedFullName, Func<object> initializer, params object[] dimensions)
            => CreateEmptyObject(assemblyQualifiedFullName, TypeRegistry, initializer, dimensions);

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="assemblyQualifiedFullName">The full name of the type to create, <see cref="Type.AssemblyQualifiedName"/></param>
        /// <param name="typeRegistry">A type registry that specifies custom mappings or factories</param>
        /// <param name="initializer">An optional initializer to use to create the object</param>
        /// <param name="dimensions">For array types, the dimensions of the array to create</param>
        /// <returns></returns>
        public object CreateEmptyObject(string assemblyQualifiedFullName, TypeRegistry typeRegistry, Func<object> initializer, params object[] dimensions)
        {
            if (string.IsNullOrEmpty(assemblyQualifiedFullName))
                throw new ArgumentNullException(nameof(assemblyQualifiedFullName));
            var type = Type.GetType(assemblyQualifiedFullName);
            return CreateEmptyObject(type.GetExtendedType(), typeRegistry, initializer, dimensions);
        }

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="type">The type to create</param>
        /// <param name="initializer">An optional initializer to use to create the object</param>
        /// <param name="dimensions">For array types, the dimensions of the array to create</param>
        /// <returns></returns>
        public object CreateEmptyObject(Type type, Func<object> initializer, params object[] dimensions)
            => CreateEmptyObject(type.GetExtendedType(), TypeRegistry, initializer, dimensions);

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="type">The type to create</param>
        /// <param name="typeRegistry">A type registry that specifies custom mappings or factories</param>
        /// <param name="dimensions">For array types, the dimensions of the array to create</param>
        /// <returns></returns>
        public object CreateEmptyObject(Type type, TypeRegistry typeRegistry, params object[] dimensions)
            => CreateEmptyObject(type.GetExtendedType(), typeRegistry, null, dimensions);

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="type">The type to create</param>
        /// <param name="typeRegistry">A type registry that specifies custom mappings or factories</param>
        /// <param name="dimensions">For array types, the dimensions of the array to create</param>
        /// <returns></returns>
        public object CreateEmptyObject(ExtendedType type, TypeRegistry typeRegistry, params object[] dimensions)
            => CreateEmptyObject(type, typeRegistry, null, dimensions);

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="typeRegistry">A type registry that specifies custom mappings or factories</param>
        /// <param name="dimensions">For array types, the dimensions of the array to create</param>
        /// <returns></returns>
        public object CreateEmptyObject(Type type, params object[] dimensions)
            => CreateEmptyObject(type.GetExtendedType(), TypeRegistry, null, dimensions);

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="typeRegistry">A type registry that specifies custom mappings or factories</param>
        /// <param name="initializer">An optional initializer to use to create the object</param>
        /// <param name="dimensions">For array types, the dimensions of the array to create</param>
        /// <returns></returns>
        public T CreateEmptyObject<T>(params object[] dimensions)
            => (T)CreateEmptyObject(typeof(T).GetExtendedType(), TypeRegistry, null, dimensions);

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="typeRegistry">A type registry that specifies custom mappings or factories</param>
        /// <param name="initializer">An optional initializer to use to create the object</param>
        /// <returns></returns>
        public T CreateEmptyObject<T>(Func<T> initializer)
        {
            Func<object> init = null;
            if (initializer != null)
                init = () => initializer();
            return (T)CreateEmptyObject(typeof(T).GetExtendedType(), TypeRegistry, init);
        }

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="typeRegistry">A type registry that specifies custom mappings or factories</param>
        /// <param name="initializer">An optional initializer to use to create the object</param>
        /// <param name="dimensions">For array types, the dimensions of the array to create</param>
        /// <returns></returns>
        public T CreateEmptyObject<T>(Func<T> initializer, params object[] dimensions)
        {
            Func<object> init = null;
            if (initializer != null)
                init = () => initializer();
            return (T)CreateEmptyObject(typeof(T), init, dimensions);
        }

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="typeRegistry">A type registry that specifies custom mappings or factories</param>
        /// <param name="initializer">An optional initializer to use to create the object</param>
        /// <param name="dimensions">For array types, the dimensions of the array to create</param>
        /// <returns></returns>
        public T CreateEmptyObject<T>(TypeRegistry typeRegistry, Func<T> initializer, params object[] dimensions)
            => (T)CreateEmptyObject(typeof(T).GetExtendedType(), typeRegistry, initializer as Func<object>, dimensions);

        /// <summary>
        /// Create a new, empty object of a given type
        /// </summary>
        /// <param name="type">The type of object to construct</param>
        /// <param name="typeRegistry">A type registry for constructing unknown types</param>
        /// <param name="typeDescriptor">A type descriptor that indicates the embedded concrete type for an interface type</param>
        /// <param name="initializer">An optional initializer to use to create the object</param>
        /// <param name="dimensions">For array types, the dimensions of the array to create</param>
        /// <returns></returns>
        public object CreateEmptyObject(ExtendedType type, TypeRegistry typeRegistry, Func<object> initializer, params object[] dimensions)
        {
            if (initializer != null)
            {
                return initializer();
            }

            // check the type registry for a custom type mapping
            if (typeRegistry?.ContainsType(type.Type) == true)
                type = typeRegistry.GetMapping(type);
            // check the type registry for a custom type factory
            if (typeRegistry?.ContainsFactoryType(type.Type) == true)
                return typeRegistry.GetFactory(type.Type).Invoke();

            // if we are asked to create an instance of an interface, try to initialize using a valid concrete type
            if (type.IsInterface && !type.IsEnumerable)
            {
                // try a known concrete type from typeSupport
                var concreteType = type.KnownConcreteTypes.FirstOrDefault();
                if (concreteType == null)
                    throw new TypeSupportException(type.Type, $"Unable to locate a concrete type for '{type.Type.FullName}'! Cannot create instance.");

                type = new ExtendedType(concreteType);
            }

            if (type.IsArray)
            {
                var createParameters = dimensions;
                // we also want to allow passing of collections/lists/arrays so do some conversion first
                if (createParameters?.Length > 0)
                {
                    var parameterType = createParameters[0].GetType();
                    if (parameterType.IsArray)
                    {
                        var ar = (Array)createParameters[0];
                        createParameters = ar.Cast<object>().ToArray();
                    }
                    else if (typeof(ICollection).IsAssignableFrom(parameterType)
                      || typeof(IList).IsAssignableFrom(parameterType))
                    {
                        var ar = (ICollection)createParameters[0];
                        createParameters = ar.Cast<object>().ToArray();
                    }
                }
                if (createParameters != null && createParameters.Length > 1)
                    createParameters.Reverse();
                if (createParameters == null || createParameters.Length == 0)
                    createParameters = new object[] { 0 };
                return Activator.CreateInstance(type.Type, createParameters);
            }
            else if (type.IsDictionary)
            {
                var genericType = type.Type.GetGenericArguments().ToList();
                Type listType;
                if (genericType.Count == 0)
                {
                    if(type.IsInterface)
                        return Activator.CreateInstance<Hashtable>() as IDictionary;
                    return Activator.CreateInstance(type.Type) as IDictionary;
                }
                else if (genericType.Count != 2)
                    throw new TypeSupportException(type.Type, "IDictionary should contain 2 element types.");
                Type[] typeArgs = { genericType[0], genericType[1] };

                listType = typeof(Dictionary<,>).MakeGenericType(typeArgs);
                return Activator.CreateInstance(listType) as IDictionary;
            }
            else if (type.IsEnumerable && !type.IsImmutable)
            {
                var genericType = type.Type.GetGenericArguments().FirstOrDefault();
                // test specifically for ICollection and List
                var isGenericCollection = type.Type.IsGenericType
                            && type.Type.GetGenericTypeDefinition() == typeof(ICollection<>);
                var isGenericList = type.Type.IsGenericType
                            && type.Type.GetGenericTypeDefinition() == typeof(List<>);
                var isGenericIList = type.Type.IsGenericType
                           && type.Type.GetGenericTypeDefinition() == typeof(IList<>);
                if (!isGenericList && !isGenericIList && !isGenericCollection)
                {
                    if (type.HasEmptyConstructor)
                    {
                        var newList = Activator.CreateInstance(type.Type);
                        return newList;
                    }
                    else if (type.Constructors.Any())
                    {
                        // special handling is required here as custom collections can't be properly
                        // initialized using FormatterServices.GetUninitializedObject()
                        var constructor = type.Constructors.First();
                        var parameters = constructor.GetParameters();
                        var param = new List<object>();
                        // initialize using defaults for constructor parameters
                        foreach (var p in parameters)
                            param.Add(CreateEmptyObject(p.ParameterType));
                        var newList = Activator.CreateInstance(type.Type, param.ToArray());
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
            else if (type.Type.ContainsGenericParameters)
            {
                // create a generic type and create an instance
                // to accomplish this, we need to create a new generic type using the type arguments from the interface
                // and the concrete class definition. voodoo!
                var originalTypeSupport = new ExtendedType(type.Type);
                var genericArguments = originalTypeSupport.Type.GetGenericArguments();
                var newType = type.Type.MakeGenericType(genericArguments);
                object newObject;
                if (type.HasEmptyConstructor)
                    newObject = Activator.CreateInstance(newType);
                else
                    newObject = FormatterServices.GetUninitializedObject(newType);
                return newObject;
            }
            else if (type.HasEmptyConstructor && !type.Type.ContainsGenericParameters)
                return Activator.CreateInstance(type.Type);
            else if (type.IsImmutable)
                return null;

            return FormatterServices.GetUninitializedObject(type.Type);
        }
    }
}
