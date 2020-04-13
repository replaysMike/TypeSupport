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
            Assert.AreEqual(1, ef.CustomAttributes.Count());
            Assert.IsNull(ef.BackedProperty);
            Assert.IsNull(ef.BackedPropertyName);
            Assert.NotNull(ef.FieldInfo);
            Assert.AreEqual(false, ef.IsBackingField);
            Assert.AreEqual(typeof(BasicObject), ef.ReflectedType);
            Assert.AreEqual(typeof(int), ef.Type);
        }

        [Test]
        public void Should_DiscoverFieldAttributes()
        {
            var fields = typeof(BasicObject).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var ef = new ExtendedField(fields.First());
            Assert.IsTrue(ef.HasAttribute<TestDecoratedAttribute>());
            Assert.IsTrue(ef.HasAttribute(typeof(TestDecoratedAttribute)));
            Assert.AreEqual(789, ef.GetAttribute<TestDecoratedAttribute>().Value);
            Assert.AreEqual(789, (ef.GetAttribute(typeof(TestDecoratedAttribute)) as TestDecoratedAttribute).Value);
        }

        [Test]
        public void Should_DiscoverAllFieldAttributes()
        {
            var fields = typeof(BasicObject).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var ef = new ExtendedField(fields.First());
            var allAttributes = ef.GetAttributes();
            Assert.AreEqual(1, allAttributes.Count());
        }

        [Test]
        public void Should_DiscoverAllFieldGenericAttributes()
        {
            var fields = typeof(BasicObject).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var ef = new ExtendedField(fields.First());
            var allAttributes = ef.GetAttributes<TestDecoratedAttribute>();
            Assert.AreEqual(1, allAttributes.Count());
        }
    }
}
