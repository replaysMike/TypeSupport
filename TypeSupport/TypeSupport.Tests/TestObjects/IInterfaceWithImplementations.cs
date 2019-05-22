#pragma warning disable 0649,0169
namespace TypeSupport.Tests.TestObjects
{
    public interface IInterfaceWithImplementations
    {
    }

    public interface IInterfaceWithGenericImplementations<T>
    {
    }

    public interface IInterfaceWithGenericImplementations<T1, T2, T3>
    {
    }

    public class InterfaceWithImplementations1 : IInterfaceWithImplementations { }
    public class InterfaceWithImplementations2 : IInterfaceWithImplementations { }
    public class InterfaceWithImplementations3 : IInterfaceWithImplementations { }
    public class InterfaceWithImplementations4 : IInterfaceWithImplementations { }
    public class InterfaceWithImplementations5 : IInterfaceWithImplementations
    {
        public int Value { get; }
        public InterfaceWithImplementations5(int value)
        {
            Value = value;
        }
    }

    public class InterfaceWithGenericImplementations<T> : IInterfaceWithGenericImplementations<T>
    {

    }

    public class InterfaceWithMultiGenericImplementations<T1, T2, T3> : IInterfaceWithGenericImplementations<T1, T2, T3>
    {

    }
}
