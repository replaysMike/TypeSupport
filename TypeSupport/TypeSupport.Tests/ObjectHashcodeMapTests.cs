using System;
using NUnit.Framework;
using TypeSupport.Extensions;
using TypeSupport.Tests.TestObjects;

namespace TypeSupport.Tests
{
    [TestFixture]
    public class ObjectHashcodeMapTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Should_Add_Hashcode()
        {
            var map = new ObjectHashcodeMap();
            var obj = new BasicObject(1);
            map.Add(obj);

            Assert.AreEqual(1, map.Count);
            map.Contains(obj);
        }

        [Test]
        public void Should_Add_Hashcode_ByHashcodeType()
        {
            var map = new ObjectHashcodeMap();
            var obj = new BasicObject(1);
            map.Add(obj.GetHashCode(), obj.GetType());

            Assert.AreEqual(1, map.Count);
            map.Contains(obj);
        }

        [Test]
        public void Should_Add_Hashcode_ByHashcodeExtendedType()
        {
            var map = new ObjectHashcodeMap();
            var obj = new BasicObject(1);
            map.Add(obj.GetHashCode(), obj.GetExtendedType());

            Assert.AreEqual(1, map.Count);
            map.Contains(obj);
        }

        [Test]
        public void ShouldNot_Add_Duplicate_Hashcode()
        {
            var map = new ObjectHashcodeMap();
            var obj = new BasicObject(1);
            map.Add(obj);
            map.Add(obj);

            Assert.AreEqual(1, map.Count);
            map.Contains(obj);
        }

        [Test]
        public void Should_Add_MultipleHashcodes()
        {
            var map = new ObjectHashcodeMap();
            var obj = new BasicObject(1);
            var obj2 = new BasicObject(2);
            var obj3 = new BasicObject(3);
            map.Add(obj);
            map.Add(obj2);
            map.Add(obj3);

            Assert.AreEqual(3, map.Count);
            map.Contains(obj);
            map.Contains(obj2);
            map.Contains(obj3);
        }

        [Test]
        public void Should_Contain_AllOverloads()
        {
            var map = new ObjectHashcodeMap();
            var obj = new BasicObject(1);
            map.Add(obj);

            Assert.AreEqual(1, map.Count);
            map.Contains(obj);
            map.Contains(obj.GetHashCode(), obj.GetType());
            map.Contains(new ObjectHashcode(obj.GetHashCode(), obj.GetType()));
        }

        [Test]
        public void Should_Throw_AddNullObject()
        {
            var map = new ObjectHashcodeMap();
            Assert.Throws<ArgumentNullException>(() => map.Add(null));
        }

        [Test]
        public void Should_Throw_AddNullType()
        {
            var map = new ObjectHashcodeMap();
            Assert.Throws<ArgumentNullException>(() => map.Add(1, (Type)null));
        }

        [Test]
        public void Should_Throw_AddNullExtendedType()
        {
            var map = new ObjectHashcodeMap();
            Assert.Throws<ArgumentNullException>(() => map.Add(1, (ExtendedType)null));
        }

        [Test]
        public void Should_Throw_ContainsNullType()
        {
            var map = new ObjectHashcodeMap();
            Assert.Throws<ArgumentNullException>(() => map.Contains(1, (Type)null));
        }

        [Test]
        public void Should_Throw_ContainsNullExtendedType()
        {
            var map = new ObjectHashcodeMap();
            Assert.Throws<ArgumentNullException>(() => map.Contains(1, (ExtendedType)null));
        }
    }
}
