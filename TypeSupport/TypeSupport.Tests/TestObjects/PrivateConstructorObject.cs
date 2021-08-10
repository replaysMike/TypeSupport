namespace TypeSupport.Tests.TestObjects
{
    public class PrivateConstructorObject<T>
    {
        private T _value;
        private PrivateConstructorObject(T value) { _value = value; }
    }
}
