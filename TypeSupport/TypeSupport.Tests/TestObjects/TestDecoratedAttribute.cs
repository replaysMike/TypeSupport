using System;

namespace TypeSupport.Tests.TestObjects
{
    public class TestDecoratedAttribute : Attribute
    {
        public int Value { get; }

        public TestDecoratedAttribute(int value)
        {
            Value = value;
        }

        public TestDecoratedAttribute()
        {
        }
    }
}
