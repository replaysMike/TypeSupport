using System;

namespace TypeSupport
{
    public class TypeConfiguration<TSource>
    {
        /// <summary>
        /// Map to a destination type
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <returns></returns>
        public TypeMap Create<TDestination>() => new TypeMap<TSource, TDestination>();

        /// <summary>
        /// Use a factory to create instance of type
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public TypeFactory<TSource, TDestination> CreateUsing<TDestination>(Func<TDestination> factory)
            => new TypeFactory<TSource, TDestination>(factory);
    }
}
