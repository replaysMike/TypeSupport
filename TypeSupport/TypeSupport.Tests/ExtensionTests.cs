using System;
using NUnit.Framework;
using TypeSupport.Extensions;

namespace TypeSupport.Tests
{
    [TestFixture]
    public class ExtensionTests
    {
        [Test]
        public void GetExtendedType_ShouldNot_WorkOnExtendedType()
        {
            var type = typeof(int).GetExtendedType(TypeSupportOptions.AllExceptCaching);
            Assert.Throws<InvalidCastException>(() => type.GetExtendedType(TypeSupportOptions.Enums));
        }

        [Test]
        public void GetExtendedType_Should_WorkOnType()
        {
            var type = typeof(int).GetExtendedType(TypeSupportOptions.AllExceptCaching);
            var newExtendedType = typeof(int).GetExtendedType(TypeSupportOptions.Enums);

            // the two types should be different
            Assert.AreEqual(type, newExtendedType.Type);
            // one should have fields, the other should not
            Assert.Greater(type.Fields.Count, 0);
            Assert.AreEqual(0, newExtendedType.Fields.Count);
        }
    }
}
