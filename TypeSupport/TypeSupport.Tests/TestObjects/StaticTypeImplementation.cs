namespace TypeSupport.Tests.TestObjects
{
    public class StaticTypeImplementation : IStaticTypeInterface
    {
        public static IStaticTypeInterface Instance { get; } = new StaticTypeImplementation();
    }

    public interface IStaticTypeInterface { }
}
