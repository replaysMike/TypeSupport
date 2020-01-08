namespace TypeSupport.Tests.TestObjects
{
#if FEATURE_CUSTOM_VALUETUPLE
    public class ObjectWithNamedTuple
    {
        public (int id, string value) TupleValue { get; }

        public ObjectWithNamedTuple()
        {
            TupleValue = (1, "test");
        }
        public ObjectWithNamedTuple((int id, string value) tuple)
        {
            TupleValue = tuple;
        }
    }
#endif
}
