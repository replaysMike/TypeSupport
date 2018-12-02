using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TypeSupport.Tests.TestObjects;

namespace TypeSupport.Tests
{
    [TestFixture]
    public class ObjectFactoryTests
    {
        [Test]
        public void Should_CreateBasicObject()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<BasicObject>();

            Assert.NotNull(instance);
            Assert.AreEqual(typeof(BasicObject), instance.GetType());
        }

        public void Should_CreateGenericList()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<IEnumerable>();

            Assert.NotNull(instance);
            Assert.AreEqual(typeof(object[]), instance.GetType());
        }


        [Test]
        public void Should_CreateIEnumerable()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<List<int>>();

            Assert.NotNull(instance);
            Assert.AreEqual(0, instance.Count);
            Assert.AreEqual(typeof(List<int>), instance.GetType());
        }

        [Test]
        public void Should_CreateGenericIList()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<IList<int>>();

            Assert.NotNull(instance);
            Assert.AreEqual(0, instance.Count);
            Assert.AreEqual(typeof(List<int>), instance.GetType());
        }

        [Test]
        public void Should_CreateGenericDictionary()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<Dictionary<int, string>>();

            Assert.NotNull(instance);
            Assert.AreEqual(0, instance.Count);
            Assert.AreEqual(typeof(Dictionary<int, string>), instance.GetType());
        }

        [Test]
        public void Should_CreateGenericIDictionary()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<IDictionary<int, string>>();

            Assert.NotNull(instance);
            Assert.AreEqual(0, instance.Count);
            Assert.AreEqual(typeof(Dictionary<int, string>), instance.GetType());
        }

        [Test]
        public void Should_CreateByteArray()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<byte[]>();

            Assert.NotNull(instance);
            Assert.AreEqual(0, instance.Length);
            Assert.AreEqual(typeof(byte[]), instance.GetType());
        }

        [Test]
        public void Should_CreateByteArrayOfLength()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<byte[]>(length: 32);

            Assert.NotNull(instance);
            Assert.AreEqual(32, instance.Length);
            Assert.AreEqual(typeof(byte[]), instance.GetType());
        }

#if FEATURE_CUSTOM_VALUETUPLE
        [Test]
        public void Should_CreateValueTuple()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<(int,string)>();

            Assert.NotNull(instance);
            Assert.AreEqual(typeof((int,string)), instance.GetType());
        }
#endif

        [Test]
        public void Should_CreateTuple()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<Tuple<int, string>>();

            Assert.NotNull(instance);
            Assert.AreEqual(typeof(Tuple<int, string>), instance.GetType());
        }

        [Test]
        public void Should_CreateNullString()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<string>();

            Assert.IsNull(instance);
        }

        [Test]
        public void Should_CreateInt()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<int>();

            Assert.NotNull(instance);
            Assert.AreEqual(typeof(int), instance.GetType());
            Assert.AreEqual(0, instance);
        }

        [Test]
        public void Should_CreateNewStringUsingInitializer()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<string>(() => string.Empty);
            Assert.AreEqual(instance.GetType(), typeof(string));
            Assert.AreEqual(instance, string.Empty);
        }

        [Test]
        public void Should_CreateNullableInt()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<int?>();

            Assert.IsNull(instance);
        }

        [Test]
        public void Should_CreateNullableIntUsingInitializer()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<int?>(() => 0);

            Assert.NotNull(instance);
            Assert.AreEqual(typeof(int), instance.GetType());
        }
    }
}
