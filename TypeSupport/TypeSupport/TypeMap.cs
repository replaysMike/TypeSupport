using System;

namespace TypeSupport
{
    /// <summary>
    /// Custom type mapping
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    public class TypeMap<TSource, TDestination> : TypeMap
    {
        public TypeMap() : base(typeof(TSource), typeof(TDestination))
        {
        }
    }

    /// <summary>
    /// Custom type mapping
    /// </summary>
    public class TypeMap
    {
        public Type Source { get; set; }
        public Type Destination { get; set; }

        internal TypeMap(Type source, Type destination)
        {
            Source = source;
            Destination = destination;
        }

        public override string ToString() => $"{Source.Name} => {Destination.Name}";
    }
}
