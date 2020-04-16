using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TypeSupport.Tests.TestObjects;
using TypeSupport.Extensions;
using System.Numerics;

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
        public void TypeSupportAndType_ShouldNot_BeEqual()
        {
            Assert.AreNotEqual(new ExtendedType(typeof(bool)), typeof(int));
        }

        [Test]
        public void TypeSupportAndType_Operator_Should_BeEqual()
        {
            Assert.IsTrue(new ExtendedType(typeof(bool)) == typeof(bool));
        }


        [Test]
        public void TypeSupportAndType_Operator_ShouldNot_BeEqual()
        {
            Assert.IsTrue(new ExtendedType(typeof(bool)) != typeof(int));
        }

        [Test]
        public void TypeSupportAndTypeSupport_Should_BeEqual()
        {
            Assert.AreEqual(new ExtendedType(typeof(bool)), new ExtendedType(typeof(bool)));
        }

        [Test]
        public void TypeSupportAndTypeSupport_Operator_Should_BeEqual()
        {
            Assert.IsTrue(new ExtendedType(typeof(bool)) == new ExtendedType(typeof(bool)));
        }

        [Test]
        public void TypeSupportAndTypeSupport_ShouldNot_BeEqual()
        {
            Assert.AreNotEqual(new ExtendedType(typeof(bool)), new ExtendedType(typeof(int)));
        }

        [Test]
        public void TypeSupportAndTypeSupport_Operator_ShouldNot_BeEqual()
        {
            Assert.IsTrue(new ExtendedType(typeof(bool)) != new ExtendedType(typeof(int)));
        }

        [Test]
        public void TypeSupport_Operators_NotNull()
        {
            Assert.IsFalse(new ExtendedType(typeof(bool)) == (ExtendedType)null);
            Assert.IsFalse(new ExtendedType(typeof(bool)) is null);
            Assert.IsFalse(new ExtendedType(typeof(bool)) == default(ExtendedType));
        }

        [Test]
        public void TypeSupport_Operators_ImplicitFromType()
        {
            ExtendedType extendedType = typeof(bool);
            Assert.NotNull(extendedType);
            Assert.AreEqual(extendedType.Type, typeof(bool));
        }

        [Test]
        public void TypeSupport_Operators_ImplicitFromExtendedType()
        {
            Type type = new ExtendedType(typeof(bool));
            Assert.NotNull(type);
            Assert.AreEqual(type, typeof(bool));
        }

        [Test]
        public void Should_All_BeNumericType()
        {
            var types = new List<Type> { typeof(char), typeof(sbyte), typeof(byte), typeof(ushort), typeof(short), typeof(uint), typeof(int), typeof(ulong), typeof(long), typeof(float), typeof(double), typeof(decimal), typeof(BigInteger),
                typeof(char?), typeof(sbyte?), typeof(byte?), typeof(ushort?), typeof(short?), typeof(uint?), typeof(int?), typeof(ulong?), typeof(long?), typeof(float?), typeof(double?), typeof(decimal?), typeof(BigInteger?),
                typeof(char[]), typeof(char?[]), typeof(int[]), typeof(int?[]), typeof(List<int>), typeof(List<int?>),
                typeof(UInt16), typeof(UInt32), typeof(UInt64), typeof(Int16), typeof(Int32), typeof(Int64) };
            foreach (var type in types)
            {
                var typeSupport = new ExtendedType(type);
                Assert.IsTrue(typeSupport.IsNumericType, $"Type {type.FullName} should be numeric but was not!");
            }
        }

        [Test]
        public void Should_None_BeNumericType()
        {
            var types = new List<Type> { typeof(bool), typeof(string), typeof(BasicObject), typeof(List<bool>), typeof(bool[]) };
            foreach (var type in types)
            {
                var typeSupport = new ExtendedType(type);
                Assert.IsFalse(typeSupport.IsNumericType, $"Type {type.FullName} should not be numeric but was!");
            }
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

        [Test]
        public void Should_Discover_NamedValueTuple_FromMethod()
        {
            var instance = (id: 1, value: "test");
            var names = InspectNamedValueTuple(instance);
            CollectionAssert.AreEqual(new List<string> { "id", "value" }, names);
        }

        private ICollection<string> InspectNamedValueTuple((int id, string value) tuple)
        {
            return tuple.GetTupleNamedParameters(System.Reflection.MethodBase.GetCurrentMethod());
        }

        [Test]
        public void Should_Discover_NamedValueTuple_FromConstructor()
        {
            var instance = new ObjectWithNamedTuple((id: 1, value: "test"));
            var tupleType = typeof((int, string));
            var names = instance.GetTupleNamedParameters(instance.GetType().GetConstructor(new Type[] { tupleType }), tupleType);
            CollectionAssert.AreEqual(new List<string> { "id", "value" }, names);
        }

        [Test]
        public void Should_Discover_NamedValueTuple_FromClass()
        {
            var instance = new ObjectWithNamedTuple();
            var names = instance.GetTupleNamedParameters(nameof(instance.TupleValue));
            CollectionAssert.AreEqual(new List<string> { "id", "value" }, names);
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

        [Test]
        public void Should_Discover_OperatorOverload()
        {
            var type = typeof(OperatorOverloadObject);
            var typeSupport = new ExtendedType(type);
            Assert.IsTrue(typeSupport.Methods.Any(x => x.Name == "op_Equality" && x.IsOperatorOverload));
            Assert.IsTrue(typeSupport.Methods.Any(x => x.Name == "op_Inequality" && x.IsOperatorOverload));
        }

        [Test]
        public void Should_Match_ImplementedInterface()
        {
            var type = typeof(InterfaceWithImplementations1);
            var typeSupport = new ExtendedType(type);
            Assert.IsTrue(typeSupport.Implements<IInterfaceWithImplementations>());
        }

        [Test]
        public void ShouldNot_Match_ImplementedInterface()
        {
            var type = typeof(InterfaceWithImplementations1);
            var typeSupport = new ExtendedType(type);
            Assert.IsFalse(typeSupport.Implements<IInterfaceWithoutImplementations>());
        }

        [Test]
        public void Should_Match_ImplementedGenericInterface()
        {
            var type = typeof(InterfaceWithGenericImplementations<bool>);
            var typeSupport = new ExtendedType(type);
            Assert.IsTrue(typeSupport.Implements<IInterfaceWithGenericImplementations<bool>>());
        }

        [Test]
        public void Should_Match_ImplementedGenericInterfaceWithoutGenericArgs()
        {
            var type = typeof(InterfaceWithGenericImplementations<bool>);
            var typeSupport = new ExtendedType(type);
            Assert.IsTrue(typeSupport.Implements(typeof(IInterfaceWithGenericImplementations<>)));
        }

        [Test]
        public void Should_Match_ImplementedMultiGenericInterfaceWithoutGenericArgs()
        {
            var type = typeof(InterfaceWithMultiGenericImplementations<bool, int, double>);
            var typeSupport = new ExtendedType(type);
            Assert.IsTrue(typeSupport.Implements(typeof(IInterfaceWithGenericImplementations<,,>)));
        }

        [Test]
        public void ShouldNot_Match_ImplementedMultiGenericInterfaceWithoutCorrectGenericArgs()
        {
            var type = typeof(InterfaceWithMultiGenericImplementations<bool, int, double>);
            var typeSupport = new ExtendedType(type);
            Assert.IsFalse(typeSupport.Implements(typeof(IInterfaceWithGenericImplementations<>)));
        }

        [Test]
        public void ShouldNot_Match_ImplementedMultiGenericInterfaceWithoutGenericArgs()
        {
            var type = typeof(InterfaceWithMultiGenericImplementations<bool, int, double>);
            var typeSupport = new ExtendedType(type);
            Assert.IsFalse(typeSupport.Implements(typeof(IInterfaceWithGenericImplementations<double, int, decimal>)));
        }

        [Test]
        public void ShouldNot_Match_ImplementedGenericInterface()
        {
            var type = typeof(InterfaceWithGenericImplementations<bool>);
            var typeSupport = new ExtendedType(type);
            Assert.IsFalse(typeSupport.Implements<IInterfaceWithGenericImplementations<double>>());
        }

        [Test]
        public void Should_GetOnlyPropertyInspect()
        {
            var instance = new ObjectWithGetProperty();
            var typeSupport = instance.GetExtendedType();
            var props = typeSupport.Properties.Select(i => i.Name);
            Assert.IsFalse(typeSupport.IsInterface);
            Assert.IsNotEmpty(props);
            foreach (var p in typeSupport.Properties)
            {
                Assert.IsNotEmpty(p.Name);
            }
        }

        [Test]
        public void Should_DiscoverTypeAttributes()
        {
            var type = typeof(BasicObject);
            var typeSupport = type.GetExtendedType();
            Assert.IsTrue(typeSupport.HasAttribute<TestDecoratedAttribute>());
            Assert.IsTrue(typeSupport.HasAttribute(typeof(TestDecoratedAttribute)));
            Assert.AreEqual(1000, typeSupport.GetAttribute<TestDecoratedAttribute>().Value);
            Assert.AreEqual(1000, (typeSupport.GetAttribute(typeof(TestDecoratedAttribute)) as TestDecoratedAttribute).Value);
        }

        [Test]
        public void Should_DiscoverBaseTypes()
        {
            var type = typeof(InheritedObject);
            var typeSupport = type.GetExtendedType();
            Assert.AreEqual(2, typeSupport.BaseTypes.Count);
            Assert.AreEqual(typeof(BaseInheritedObject), typeSupport.BaseTypes.First());
            Assert.AreEqual(typeof(object), typeSupport.BaseTypes.Skip(1).First());
        }
    }
}
