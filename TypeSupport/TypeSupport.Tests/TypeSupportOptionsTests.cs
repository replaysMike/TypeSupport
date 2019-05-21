using NUnit.Framework;
using System;

namespace TypeSupport.Tests
{
    [TestFixture]
    public class TypeSupportOptionsTests
    {
        [Test]
        public void Should_TypeSupportOptions_AllBeCorrect()
        {
            var allOptions = TypeSupportOptions.Attributes
                | TypeSupportOptions.Caching
                | TypeSupportOptions.Collections
                | TypeSupportOptions.ConcreteTypes
                | TypeSupportOptions.Constructors
                | TypeSupportOptions.Enums
                | TypeSupportOptions.Fields
                | TypeSupportOptions.Generics
                | TypeSupportOptions.Indexers
                | TypeSupportOptions.Properties
                | TypeSupportOptions.Methods;
            Assert.AreEqual(allOptions, TypeSupportOptions.All);
        }

         [Test]
        public void Should_TypeSupportOptions_PowerOfTwos()
        {
            var values = Enum.GetValues(typeof(TypeSupportOptions));
            foreach(int value in values)
            {
                if ((TypeSupportOptions)value != TypeSupportOptions.All && value > 1)
                    Assert.AreEqual(0, value % 2, $"Value is invalid enum flags value {value} ({(TypeSupportOptions)value})");
            }
        }
    }
}
