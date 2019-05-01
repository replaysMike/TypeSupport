#pragma warning disable 0649,0169
namespace TypeSupport.Tests.TestObjects
{
    public interface IInterfaceWithImplementations
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
}
