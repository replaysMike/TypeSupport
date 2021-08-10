using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TypeSupport.Extensions;
using TypeSupport.Tests.TestObjects;

namespace TypeSupport.Tests
{
    [TestFixture]
    public class AttributeInspectionTests
    {
        [Test]
        public void Should_Inspect_AttributesWithParams()
        {
            var obj = new BasicObjectWithParamAttributes();
            var extendedType = obj.GetExtendedType();
            var factory = new ObjectFactory();
            var test = factory.CreateEmptyObject(extendedType);
            var inspector = new TypeInspector(extendedType, TypeSupportOptions.All);

            Assert.AreEqual(2, extendedType.Attributes.Count);
        }
    }
}
