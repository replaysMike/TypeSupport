using NUnit.Framework;
using System.Linq;
using System.Reflection;
using TypeSupport.Tests.TestObjects;

namespace TypeSupport.Tests
{
    [TestFixture]
    public class ExtendedPropertyTests
    {
        [Test]
        public void Should_CreateExtendedProperty()
        {
            var properties = typeof(BasicObject).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var ef = new ExtendedProperty(properties.First());
            Assert.NotNull(ef);
            Assert.IsNotEmpty(ef.Name);
            Assert.AreEqual(1, ef.CustomAttributes.Count());
            Assert.AreEqual("<Id>k__BackingField", ef.BackingFieldName);
            Assert.IsTrue(ef.HasGetMethod);
            Assert.NotNull(ef.GetMethod);
            Assert.IsTrue(ef.HasSetMethod);
            Assert.NotNull(ef.SetMethod);
            Assert.IsTrue(ef.IsAutoProperty);
            Assert.NotNull(ef.PropertyInfo);
            Assert.AreEqual(typeof(BasicObject), ef.ReflectedType);
            Assert.AreEqual(typeof(int), ef.Type);
            ;
        }

        [Test]
        public void Should_DiscoverPropertyAttributes()
        {
            var properties = typeof(BasicObject).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var ef = new ExtendedProperty(properties.First());
            Assert.IsTrue(ef.HasAttribute<TestDecoratedAttribute>());
            Assert.IsTrue(ef.HasAttribute(typeof(TestDecoratedAttribute)));
            Assert.AreEqual(123, ef.GetAttribute<TestDecoratedAttribute>().Value);
            Assert.AreEqual(123, (ef.GetAttribute(typeof(TestDecoratedAttribute)) as TestDecoratedAttribute).Value);
        }

        [Test]
        public void Should_DiscoverAllPropertyAttributes()
        {
            var fields = typeof(BasicObject).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var ef = new ExtendedProperty(fields.First());
            var allAttributes = ef.GetAttributes();
            Assert.AreEqual(1, allAttributes.Count());
        }

        [Test]
        public void Should_DiscoverAllPropertyGenericAttributes()
        {
            var fields = typeof(BasicObject).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var ef = new ExtendedProperty(fields.First());
            var allAttributes = ef.GetAttributes<TestDecoratedAttribute>();
            Assert.AreEqual(1, allAttributes.Count());
        }
    }
}
