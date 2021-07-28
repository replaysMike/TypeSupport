using System;

namespace TypeSupport
{
    /// <summary>
    /// Type factory
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    public class TypeFactory<TSource, TDestination> : TypeFactory
    {
        /// <summary>
        /// Generic factory method
        /// </summary>
        public new Func<TDestination> Factory { get; }

        public TypeFactory(Func<TDestination> factory) : base(typeof(TSource), factory as Func<object>)
        {
            Factory = factory;
        }
    }

    /// <summary>
    /// Type factory
    /// </summary>
    public class TypeFactory
    {
        /// <summary>
        /// The source type
        /// </summary>
        public Type Source { get; }
        /// <summary>
        /// The factory method
        /// </summary>
        public Func<object> Factory { get; }
        internal TypeFactory(Type source)
        {
            Source = source;
        }
        internal TypeFactory(Type source, Func<object> factory)
        {
            Source = source;
            Factory = factory;
        }
    }
}
