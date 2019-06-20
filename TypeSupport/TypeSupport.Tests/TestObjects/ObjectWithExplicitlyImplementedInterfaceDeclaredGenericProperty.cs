namespace TypeSupport.Tests.TestObjects
{

    public interface IValue<T>
    {
        T Value { get; }
    }

    public class ObjectWithExplicitlyImplementedInterfaceDeclaredGenericProperty<T> : IValue<T>
    {
        T IValue<T>.Value { get; }
    }
}
