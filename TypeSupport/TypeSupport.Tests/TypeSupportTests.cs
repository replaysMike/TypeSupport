using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TypeSupport.Tests.TestObjects;

namespace TypeSupport.Tests
{
    [TestFixture]
    public class TypeSupportTests
    {
        [Test]
        public void TypeSupportAndType_Should_BeEqual()
        {
            Assert.AreEqual(new TypeSupport(typeof(bool)), typeof(bool));
        }

        [Test]
        public void TypeSupportAndTypeSupport_Should_BeEqual()
        {
            Assert.AreEqual(new TypeSupport(typeof(bool)), new TypeSupport(typeof(bool)));
        }

        [Test]
        public void TypeSupportAndTypeSupport_ShouldNot_BeEqual()
        {
            Assert.AreNotEqual(new TypeSupport(typeof(bool)), new TypeSupport(typeof(int)));
        }

        [Test]
        public void Should_Create_TypeSupport()
        {
            var type = typeof(object);
            var typeSupport = new TypeSupport(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(type, typeSupport);
        }

        [Test]
        public void Should_Create_TypeSupportIntegral()
        {
            var type = typeof(int);
            var typeSupport = new TypeSupport(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(type, typeSupport);
        }

        [Test]
        public void Should_Create_TypeSupportCustomObject()
        {
            var type = typeof(BasicObject);
            var typeSupport = new TypeSupport(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(type, typeSupport);
        }

        [Test]
        public void Should_Discover_Interface()
        {
            var type = typeof(IInterfaceWithImplementations);
            var typeSupport = new TypeSupport(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsInterface);
        }

        [Test]
        public void Should_Discover_TypeWithConcreteClasses()
        {
            var type = typeof(IInterfaceWithImplementations);
            var typeSupport = new TypeSupport(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(4, typeSupport.KnownConcreteTypes.Count);
        }

        [Test]
        public void Should_Discover_TypeWithoutConcreteClasses()
        {
            var type = typeof(IInterfaceWithoutImplementations);
            var typeSupport = new TypeSupport(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(0, typeSupport.KnownConcreteTypes.Count);
        }

        [Test]
        public void Should_Discover_NoEmptyConstuctor()
        {
            var type = typeof(BasicObject);
            var typeSupport = new TypeSupport(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(false, typeSupport.HasEmptyConstructor);
        }

        [Test]
        public void Should_Discover_HasEmptyConstuctor()
        {
            var type = typeof(BasicObjectWithConstructor);
            var typeSupport = new TypeSupport(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.HasEmptyConstructor);
        }

        [Test]
        public void Should_Discover_Tuple()
        {
            var type = typeof(Tuple<int, double>);
            var typeSupport = new TypeSupport(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsTuple);
            Assert.AreEqual(2, typeSupport.GenericArgumentTypes.Count);
            Assert.AreEqual(typeof(int), typeSupport.GenericArgumentTypes.First());
            Assert.AreEqual(typeof(double), typeSupport.GenericArgumentTypes.Skip(1).First());
        }

        [Test]
        public void Should_Discover_ValueTuple()
        {
            var type = typeof((int, double));
            var typeSupport = new TypeSupport(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsValueTuple);
            Assert.AreEqual(2, typeSupport.GenericArgumentTypes.Count);
            Assert.AreEqual(typeof(int), typeSupport.GenericArgumentTypes.First());
            Assert.AreEqual(typeof(double), typeSupport.GenericArgumentTypes.Skip(1).First());
        }

        [Test]
        public void Should_Discover_GenericCollection()
        {
            var type = typeof(ICollection<int>);
            var typeSupport = new TypeSupport(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsCollection);
            Assert.AreEqual(1, typeSupport.GenericArgumentTypes.Count);
            Assert.AreEqual(typeof(int), typeSupport.GenericArgumentTypes.First());
        }

        [Test]
        public void Should_Discover_GenericDictionary()
        {
            var type = typeof(IDictionary<int, double>);
            var typeSupport = new TypeSupport(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsDictionary);
            Assert.AreEqual(2, typeSupport.GenericArgumentTypes.Count);
            Assert.AreEqual(typeof(int), typeSupport.GenericArgumentTypes.First());
            Assert.AreEqual(typeof(double), typeSupport.GenericArgumentTypes.Skip(1).First());
        }

        [Test]
        public void Should_Discover_Enum()
        {
            var type = typeof(DayOfWeek);
            var typeSupport = new TypeSupport(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsEnum);
            Assert.AreEqual(7, typeSupport.EnumValues.Count);
        }

        [Test]
        public void Should_Discover_NullableInt()
        {
            var type = typeof(int?);
            var typeSupport = new TypeSupport(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsNullable);
            Assert.AreEqual(typeof(int), typeSupport.NullableBaseType);
        }

        [Test]
        public void Should_Discover_Array()
        {
            var type = typeof(byte[]);
            var typeSupport = new TypeSupport(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsArray);
            Assert.AreEqual(typeof(byte), typeSupport.ElementType);
        }

        [Test]
        public void Should_Discover_EnumerableTypes()
        {
            var type = typeof(List<int>);
            var typeSupport = new TypeSupport(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsEnumerable);
            Assert.AreEqual(typeof(int), typeSupport.ElementType);
        }
    }
}
