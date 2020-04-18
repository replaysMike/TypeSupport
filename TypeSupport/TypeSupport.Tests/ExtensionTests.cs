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
            var type = typeof(int).GetExtendedType();
            var newExtendedType = type.GetExtendedType(TypeSupportOptions.Enums);

            // we requested to inspect enums only, but we should get back the same extended type
            Assert.AreEqual(type, newExtendedType);
            // sanity check
            var type2 = typeof(long).GetExtendedType();
            Assert.AreNotEqual(type2, newExtendedType);
            // both should have fields if they are the same instance
            Assert.Greater(type.Fields.Count, 0);
            Assert.Greater(newExtendedType.Fields.Count, 0);
        }

        [Test]
        public void GetExtendedType_Should_WorkOnType()
        {
            var type = typeof(int).GetExtendedType();
            var newExtendedType = typeof(int).GetExtendedType(TypeSupportOptions.Enums);

            // the two types should be different
            Assert.AreEqual(type, newExtendedType.Type);
            // one should have fields, the other should not
            Assert.Greater(type.Fields.Count, 0);
            Assert.AreEqual(0, newExtendedType.Fields.Count);
        }
    }
}
