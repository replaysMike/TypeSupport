#pragma warning disable 0649,0169
using System;

namespace TypeSupport.Tests.TestObjects
{
    public struct StructObject : IEquatable<StructObject>
    {
        public int A { get; }
        public int B { get; }
        public string C { get; }
        public StructObject(int a, int b, string c)
        {
            A = a;
            B = b;
            C = c;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(StructObject))
                return false;
            return base.Equals((StructObject)obj);
        }

        public bool Equals(StructObject other)
        {
            return A == other.A && B == other.B && C == other.C;
        }
    }
}
