using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TypeSupport.Tests.TestObjects;
using TypeSupport.Extensions;

namespace TypeSupport.Tests
{
    [TestFixture]
    public class TypeSupportTests
    {
        [Test]
        public void TypeSupportAndType_Should_BeEqual()
        {
            Assert.AreEqual(new ExtendedType(typeof(bool)), typeof(bool));
        }

        [Test]
        public void TypeSupportAndTypeSupport_Should_BeEqual()
        {
            Assert.AreEqual(new ExtendedType(typeof(bool)), new ExtendedType(typeof(bool)));
        }

        [Test]
        public void TypeSupportAndTypeSupport_ShouldNot_BeEqual()
        {
            Assert.AreNotEqual(new ExtendedType(typeof(bool)), new ExtendedType(typeof(int)));
        }

        [Test]
        public void Should_Create_TypeSupport()
        {
            var type = typeof(object);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(type, typeSupport);
        }

        [Test]
        public void Should_Create_TypeSupportIntegral()
        {
            var type = typeof(int);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(type, typeSupport);
        }

        [Test]
        public void Should_Create_TypeSupportCustomObject()
        {
            var type = typeof(BasicObject);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(type, typeSupport);
        }

        [Test]
        public void Should_Discover_Interface()
        {
            var type = typeof(IInterfaceWithImplementations);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsInterface);
        }

        [Test]
        public void Should_Discover_TypeWithConcreteClasses()
        {
            var type = typeof(IInterfaceWithImplementations);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(5, typeSupport.KnownConcreteTypes.Count);
        }

        [Test]
        public void Should_Discover_TypeWithoutConcreteClasses()
        {
            var type = typeof(IInterfaceWithoutImplementations);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(0, typeSupport.KnownConcreteTypes.Count);
        }

        [Test]
        public void Should_Discover_NoEmptyConstuctor()
        {
            var type = typeof(BasicObject);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(false, typeSupport.HasEmptyConstructor);
        }

        [Test]
        public void Should_Discover_HasEmptyConstuctor()
        {
            var type = typeof(BasicObjectWithConstructor);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.HasEmptyConstructor);
        }

        [Test]
        public void Should_Discover_Tuple()
        {
            var type = typeof(Tuple<int, double>);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsTuple);
            Assert.AreEqual(2, typeSupport.GenericArgumentTypes.Count);
            Assert.AreEqual(typeof(int), typeSupport.GenericArgumentTypes.First());
            Assert.AreEqual(typeof(double), typeSupport.GenericArgumentTypes.Skip(1).First());
        }

#if FEATURE_CUSTOM_VALUETUPLE
        [Test]
        public void Should_Discover_ValueTuple()
        {
            var type = typeof((int, double));
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsValueTuple);
            Assert.AreEqual(2, typeSupport.GenericArgumentTypes.Count);
            Assert.AreEqual(typeof(int), typeSupport.GenericArgumentTypes.First());
            Assert.AreEqual(typeof(double), typeSupport.GenericArgumentTypes.Skip(1).First());
        }
#endif

        [Test]
        public void Should_Discover_GenericIList()
        {
            var type = typeof(IList<int>);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsCollection);
            Assert.AreEqual(1, typeSupport.GenericArgumentTypes.Count);
            Assert.AreEqual(typeof(int), typeSupport.GenericArgumentTypes.First());
        }

        [Test]
        public void Should_Discover_IList()
        {
            var type = typeof(IList);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsCollection);
            Assert.AreEqual(typeof(object), typeSupport.ElementType);
        }

        [Test]
        public void Should_Discover_GenericCollection()
        {
            var type = typeof(ICollection<int>);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsCollection);
            Assert.AreEqual(1, typeSupport.GenericArgumentTypes.Count);
            Assert.AreEqual(typeof(int), typeSupport.GenericArgumentTypes.First());
        }

        [Test]
        public void Should_Discover_Collection()
        {
            var type = typeof(ICollection);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsCollection);
            Assert.AreEqual(1, typeSupport.GenericArgumentTypes.Count);
            Assert.AreEqual(typeof(object), typeSupport.GenericArgumentTypes.First());
        }

        [Test]
        public void Should_Discover_GenericDictionary()
        {
            var type = typeof(IDictionary<int, double>);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsDictionary);
            Assert.AreEqual(2, typeSupport.GenericArgumentTypes.Count);
            Assert.AreEqual(typeof(int), typeSupport.GenericArgumentTypes.First());
            Assert.AreEqual(typeof(double), typeSupport.GenericArgumentTypes.Skip(1).First());
        }

        [Test]
        public void Should_Discover_GenericDictionaryDuplicateTypes()
        {
            var type = typeof(IDictionary<int, int>);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsDictionary);
            Assert.AreEqual(2, typeSupport.GenericArgumentTypes.Count);
            Assert.AreEqual(typeof(int), typeSupport.GenericArgumentTypes.First());
            Assert.AreEqual(typeof(int), typeSupport.GenericArgumentTypes.Skip(1).First());
        }

        [Test]
        public void Should_Discover_Dictionary()
        {
            var type = typeof(IDictionary);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsDictionary);
            Assert.AreEqual(1, typeSupport.GenericArgumentTypes.Count);
            Assert.AreEqual(typeof(object), typeSupport.GenericArgumentTypes.First());
        }

        [Test]
        public void Should_Discover_Enum()
        {
            var type = typeof(DayOfWeek);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsEnum);
            Assert.AreEqual(7, typeSupport.EnumValues.Count);
        }

        [Test]
        public void Should_Discover_ReferenceType()
        {
            var type = typeof(object);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsReferenceType);
        }

        [Test]
        public void Should_Discover_Struct()
        {
            var type = typeof(StructObject);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsStruct);
        }

        [Test]
        public void Should_Discover_NullableInt()
        {
            var type = typeof(int?);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsNullable);
            Assert.AreEqual(typeof(int), typeSupport.NullableBaseType);
        }

        [Test]
        public void Should_Discover_Array()
        {
            var type = typeof(byte[]);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsArray);
            Assert.AreEqual(typeof(byte), typeSupport.ElementType);
        }

        [Test]
        public void Should_Discover_EnumerableTypes()
        {
            var type = typeof(List<int>);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsEnumerable);
            Assert.AreEqual(typeof(int), typeSupport.ElementType);
        }

        [Test]
        public void Should_Discover_KeyValuePairTypes()
        {
            var type = typeof(KeyValuePair<int, string>);
            var typeSupport = new ExtendedType(type);

            Assert.NotNull(typeSupport);
            Assert.AreEqual(true, typeSupport.IsKeyValuePair);
            Assert.AreEqual(true, typeSupport.IsGeneric);
            Assert.AreEqual(typeof(int), typeSupport.ElementType);
            Assert.AreEqual(typeof(int), typeSupport.GenericArgumentTypes.First());
            Assert.AreEqual(typeof(string), typeSupport.GenericArgumentTypes.Skip(1).First());
        }

        [Test]
        public void Should_Discover_AutoProperties()
        {
            var type = typeof(BasicObject);
            var properties = type.GetProperties(PropertyOptions.All);

            Assert.AreEqual(1, properties.Where(x => x.IsAutoProperty).Count());
            Assert.AreEqual(true, properties.First().IsAutoProperty);
            Assert.AreEqual(false, string.IsNullOrEmpty(properties.First().BackingFieldName));
        }

        [Test]
        public void Should_Discover_AutoPropertyFields()
        {
            var type = typeof(BasicObject);
            var fields = type.GetFields(FieldOptions.BackingFields);

            Assert.AreEqual(1, fields.Count);
            Assert.AreEqual(true, fields.First().IsBackingField);
            Assert.AreEqual(false, string.IsNullOrEmpty(fields.First().BackedPropertyName));
        }

        [Test]
        public void Should_Discover_AutoPropertyFieldsOnAnonymousTypes()
        {
            var obj1 = new { Field1 = "Test" };
            var fields = obj1.GetFields(FieldOptions.BackingFields);
            Assert.AreEqual(1, fields.Count);
            Assert.AreEqual(true, fields.First().IsBackingField);
            Assert.AreEqual(false, string.IsNullOrEmpty(fields.First().BackedPropertyName));
        }

        [Test]
        public void Should_Discover_AnonymousTypes()
        {
            var anonymous = new { Test1 = 1, Test2 = "string" };
            var type = anonymous.GetType();
            var typeSupport = new ExtendedType(type);

            Assert.AreEqual(true, typeSupport.IsAnonymous);
        }

        [Test]
        public void Should_Discover_ReadOnlyFields()
        {
            var type = typeof(ReadOnlyObject);
            var fields = type.GetFields(FieldOptions.All);

            Assert.AreEqual(1, fields.Count);
        }

        [Test]
        public void Should_Discover_InheritedFields()
        {
            var type = typeof(InheritedObject);
            var fields = type.GetFields(FieldOptions.All);

            // there are 4 fields, plus an overridden field with duplicate name
            Assert.AreEqual(5, fields.Count);
        }

        [Test]
        public void Should_Throw_DuplicateInheritedPropertyNames()
        {
            var obj = new InheritedObject();
            Assert.Throws<System.Reflection.AmbiguousMatchException>(() => {
                var property = obj.GetProperty("Id");
            });
        }

        [Test]
        public void Should_Locate_DuplicateInheritedPropertyNamesByPropertyType()
        {
            var obj = new InheritedObject();
            var property = obj.GetProperty("Id", typeof(int?));
            Assert.AreEqual(typeof(int?), property.PropertyType);
        }

        [Test]
        public void Should_Locate_DuplicateInheritedPropertyNamesByClassOwner()
        {
            var obj = new InheritedObject();
            var property = obj.GetProperty("Id", typeof(BaseInheritedObject));
            Assert.AreEqual(typeof(int?), property.PropertyType);
        }

        [Test]
        public void Should_Discover_InheritedProperties()
        {
            var type = typeof(InheritedObject);
            var properties = type.GetProperties(PropertyOptions.All);

            Assert.AreEqual(3, properties.Count);
        }

        [Test]
        public void Should_Succeeed_EnumBitwiseFlagSupport()
        {
            var options = PropertyOptions.HasGetter | PropertyOptions.HasSetter;
            Assert.IsTrue(options.BitwiseHasFlag<PropertyOptions>(PropertyOptions.HasSetter));
            Assert.IsTrue(options.BitwiseHasFlag<PropertyOptions>(PropertyOptions.HasGetter));
            Assert.IsFalse(options.BitwiseHasFlag<PropertyOptions>(PropertyOptions.Private));
            Assert.IsFalse(options.BitwiseHasFlag<PropertyOptions>(PropertyOptions.Public));
        }
    }
}
