using NUnit.Framework;
using System.Linq;
using System.Reflection;
using TypeSupport.Tests.TestObjects;

namespace TypeSupport.Tests
{
    [TestFixture]
    public class ExtendedFieldTests
    {
        [Test]
        public void Should_CreateExtendedField()
        {
            var fields = typeof(BasicObject).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var ef = new ExtendedField(fields.First());
            Assert.NotNull(ef);
            Assert.IsNotEmpty(ef.Name);
            Assert.AreEqual(0, ef.CustomAttributes.Count());
            Assert.IsNull(ef.BackedProperty);
            Assert.IsNull(ef.BackedPropertyName);
            Assert.NotNull(ef.FieldInfo);
            Assert.AreEqual(false, ef.IsBackingField);
            Assert.AreEqual(typeof(BasicObject), ef.ReflectedType);
            Assert.AreEqual(typeof(int), ef.Type);
        }
    }
}
