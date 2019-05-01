using NUnit.Framework;
using System;
using TypeSupport.Tests.TestObjects;

namespace TypeSupport.Tests
{
    [TestFixture]
    public class TypeRegistryTests
    {
        [Test]
        public void Should_CreateRegistryMapping()
        {
            var registry = TypeRegistry.Configure(config => {
                config.AddMapping<IInterfaceWithImplementations, InterfaceWithImplementations1>();
            });

            Assert.AreEqual(0, registry.Factories.Count);
            Assert.AreEqual(1, registry.Mappings.Count);
            Assert.IsTrue(registry.ContainsType(typeof(IInterfaceWithImplementations)));
        }

        [Test]
        public void Should_CreateTypeMap()
        {
            var typeMap = new TypeConfiguration<int>().Create<string>();

            Assert.AreEqual(typeof(int), typeMap.Source);
            Assert.AreEqual(typeof(string), typeMap.Destination);
        }

        [Test]
        public void Should_CreateTypeFactory()
        {
            var typeFactory = new TypeConfiguration<int>().CreateUsing(() => "value");

            Assert.AreEqual(typeof(int), typeFactory.Source);
            Assert.NotNull(typeFactory.Factory);
            Assert.AreEqual(typeof(Func<string>), typeFactory.Factory.GetType());
        }

        [Test]
        public void Should_CreateRegistryFactory()
        {
            var registry = TypeRegistry.Configure(config => {
                config.AddFactory<IInterfaceWithImplementations, InterfaceWithImplementations1>(() => new InterfaceWithImplementations1());
            });

            Assert.AreEqual(0, registry.Mappings.Count);
            Assert.AreEqual(1, registry.Factories.Count);
            Assert.IsTrue(registry.ContainsFactoryType(typeof(IInterfaceWithImplementations)));
        }

        [Test]
        public void Should_CreateObjectFromFactory()
        {
            var testValue = 123;
            var registry = TypeRegistry.Configure(config => {
                config.AddFactory<IInterfaceWithImplementations, InterfaceWithImplementations5>(() => new InterfaceWithImplementations5(testValue));
            });

            Assert.AreEqual(1, registry.Factories.Count);
            var factory = new ObjectFactory(registry);
            var emptyObject = factory.CreateEmptyObject<IInterfaceWithImplementations>();
            Assert.NotNull(emptyObject);
            Assert.AreEqual(typeof(InterfaceWithImplementations5), emptyObject.GetType());
            // ensure our factory created the correct class with a value we defined
            Assert.AreEqual(testValue, ((InterfaceWithImplementations5)emptyObject).Value);
        }
    }
}
