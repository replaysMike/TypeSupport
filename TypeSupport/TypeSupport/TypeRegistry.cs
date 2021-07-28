using System;
using System.Collections.Generic;
using System.Linq;
using TypeSupport.Extensions;

namespace TypeSupport
{
    /// <summary>
    /// A type registry to map one type to another
    /// </summary>
    public class TypeRegistry
    {
        /// <summary>
        /// Gets the registered type mappings
        /// </summary>
        public ICollection<TypeMap> Mappings { get; private set; }
        /// <summary>
        /// Gets the registered type factories
        /// </summary>
        public ICollection<TypeFactory> Factories { get; private set; }

        private TypeRegistry()
        {
            Mappings = new List<TypeMap>();
            Factories = new List<TypeFactory>();
        }

        internal TypeRegistry(TypeMap[] typeMaps)
        {
            Mappings = new List<TypeMap>();
            foreach (var typeMap in typeMaps)
                Mappings.Add(typeMap);
        }

        /// <summary>
        /// Add a type mapping from source type to destination type
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        public void AddMapping<TSource, TDestination>()
            => Mappings.Add(new TypeMap<TSource, TDestination>());

        /// <summary>
        /// Add a type factory for creating types
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="factory"></param>
        public void AddFactory<TSource, TDestination>(Func<TDestination> factory)
            => Factories.Add(new TypeFactory<TSource, TDestination>(factory));

        /// <summary>
        /// True if a mapping exists for the source type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool ContainsType(Type type) => Mappings.Any(x => x.Source.Equals(type));

        /// <summary>
        /// True if a mapping exists for the source type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool ContainsType(ExtendedType type) => Mappings.Any(x => x.Source.Equals(type.Type));

        /// <summary>
        /// True if a factory exists for the source type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool ContainsFactoryType(Type type) => Factories.Any(x => x.Source.Equals(type));

        /// <summary>
        /// True if a factory exists for the source type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool ContainsFactoryType(ExtendedType type) => ContainsFactoryType(type.Type);

        /// <summary>
        /// Get the destination mapping for a source type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal Type GetMapping(Type type)
        {
            var mapping = Mappings
                .Where(x => x.Source.Equals(type))
                .Select(x => x.Destination)
                .FirstOrDefault();
            return mapping;
        }

        /// <summary>
        /// Get the destination mapping for a source type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal ExtendedType GetMapping(ExtendedType type)
        {
            var mapping = Mappings
                .Where(x => x.Source.Equals(type))
                .Select(x => x.Destination)
                .FirstOrDefault()
                .GetExtendedType();
            return mapping;
        }

        /// <summary>
        /// Get the factory for creating a source type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal Func<object> GetFactory(Type type)
        {
            var factory = Factories.Where(x => x.Source.Equals(type)).Select(x => x.Factory).FirstOrDefault();
            return factory;
        }

        /// <summary>
        /// Get the factory for creating a source type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal Func<object> GetFactory(ExtendedType type) => GetFactory(type.Type);

        /// <summary>
        /// Configure a new type registry
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static TypeRegistry Configure(Action<TypeRegistry> config)
        {
            var registry = new TypeRegistry();
            config(registry);
            return registry;
        }

        /// <summary>
        /// For the source type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static TypeConfiguration<T> For<T>() => new TypeConfiguration<T>();
    }
}
