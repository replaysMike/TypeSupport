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
        public void Should_CreateNonGenericIDictionary()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<IDictionary>();

            Assert.NotNull(instance);
            Assert.AreEqual(0, instance.Count);
            Assert.IsTrue(instance is IDictionary);
        }

        [Test]
        public void Should_CreateEmptyByteArray()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<byte[]>();

            Assert.NotNull(instance);
            Assert.AreEqual(0, instance.Length);
            Assert.AreEqual(typeof(byte[]), instance.GetType());
        }

        [Test]
        public void Should_CreatePopulatedByteArray()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<byte[]>(10);

            Assert.NotNull(instance);
            Assert.AreEqual(10, instance.Length);
            Assert.AreEqual(typeof(byte[]), instance.GetType());
        }

        [Test]
        public void Should_CreatePopulatedMultidimensionalByteArray()
        {
            var factory = new ObjectFactory();
            var testArray = new byte[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
            var instance = factory.CreateEmptyObject<byte[,]>(new object[] { 2, 3 });

            Assert.NotNull(instance);
            Assert.AreEqual(typeof(byte[,]), instance.GetType());
            Assert.AreEqual(testArray.Rank, instance.Rank);
            Assert.AreEqual(testArray.GetLength(0), instance.GetLength(0));
            Assert.AreEqual(testArray.GetLength(1), instance.GetLength(1));
        }

        [Test]
        public void Should_CreatePopulatedMultidimensionalByteArrayViaIntArrayDimensions()
        {
            var factory = new ObjectFactory();
            var testArray = new byte[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
            var instance = factory.CreateEmptyObject<byte[,]>(new int[] { 2, 3 });

            Assert.NotNull(instance);
            Assert.AreEqual(typeof(byte[,]), instance.GetType());
            Assert.AreEqual(testArray.Rank, instance.Rank);
            Assert.AreEqual(testArray.GetLength(0), instance.GetLength(0));
            Assert.AreEqual(testArray.GetLength(1), instance.GetLength(1));
        }

        [Test]
        public void Should_CreatePopulatedMultidimensionalByteArrayViaIntDimensions()
        {
            var factory = new ObjectFactory();
            var testArray = new byte[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
            var instance = factory.CreateEmptyObject<byte[,]>(2, 3);

            Assert.NotNull(instance);
            Assert.AreEqual(typeof(byte[,]), instance.GetType());
            Assert.AreEqual(testArray.Rank, instance.Rank);
            Assert.AreEqual(testArray.GetLength(0), instance.GetLength(0));
            Assert.AreEqual(testArray.GetLength(1), instance.GetLength(1));
        }

        [Test]
        public void Should_CreatePopulatedMultidimensionalByteArrayViaList()
        {
            var factory = new ObjectFactory();
            var testArray = new byte[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
            var instance = factory.CreateEmptyObject<byte[,]>(new List<int> { 2, 3 });

            Assert.NotNull(instance);
            Assert.AreEqual(typeof(byte[,]), instance.GetType());
            Assert.AreEqual(testArray.Rank, instance.Rank);
            Assert.AreEqual(testArray.GetLength(0), instance.GetLength(0));
            Assert.AreEqual(testArray.GetLength(1), instance.GetLength(1));
        }

        [Test]
        public void Should_CreateByteArrayOfLength()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<byte[]>(32);

            Assert.NotNull(instance);
            Assert.AreEqual(32, instance.Length);
            Assert.AreEqual(typeof(byte[]), instance.GetType());
        }

        [Test]
        public void Should_CreateEmptyHashtable()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<Hashtable>();

            Assert.NotNull(instance);
            Assert.AreEqual(0, instance.Count);
            Assert.AreEqual(typeof(Hashtable), instance.GetType());
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
        public void Should_CreateGenericSingularTuple()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<Tuple<int>>();

            Assert.NotNull(instance);
            Assert.AreEqual(typeof(Tuple<int>), instance.GetType());
        }

        [Test]
        public void Should_CreateSingularTuple()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject(typeof(Tuple<int>));

            Assert.NotNull(instance);
            Assert.AreEqual(typeof(Tuple<int>), instance.GetType());
        }

        [Test]
        public void Should_CreateGenericTuple()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject<Tuple<int, string>>();

            Assert.NotNull(instance);
            Assert.AreEqual(typeof(Tuple<int, string>), instance.GetType());
        }

        [Test]
        public void Should_CreateNonGenericTuple()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject(typeof(Tuple<int, string>));

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

        [Test]
        public void Should_CreateObjectWithExplicitlyImplementedInterfaceDeclaredGenericProperty()
        {
            var factory = new ObjectFactory();
            var instance = factory.CreateEmptyObject(typeof(ObjectWithExplicitlyImplementedInterfaceDeclaredGenericProperty<int>));

            Assert.NotNull(instance);
            Assert.AreEqual(
                typeof(ObjectWithExplicitlyImplementedInterfaceDeclaredGenericProperty<int>),
                instance.GetType());
        }
    }
}
