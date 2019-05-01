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
            Assert.AreEqual(0, ef.CustomAttributes.Count());
            Assert.AreEqual("<Id>k__BackingField", ef.BackingFieldName);
            Assert.IsTrue(ef.HasGetMethod);
            Assert.NotNull(ef.GetMethod);
            Assert.IsTrue(ef.HasSetMethod);
            Assert.NotNull(ef.SetMethod);
            Assert.IsTrue(ef.IsAutoProperty);
            Assert.NotNull(ef.PropertyInfo);
            Assert.AreEqual(typeof(BasicObject), ef.ReflectedType);
            Assert.AreEqual(typeof(int), ef.Type);
        }
    }
}
