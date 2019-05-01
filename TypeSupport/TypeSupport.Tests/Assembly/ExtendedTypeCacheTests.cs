using NUnit.Framework;
using System;
using TypeSupport.Tests.TestObjects;

namespace TypeSupport.Tests.Assembly
{
    [TestFixture]
    public class ExtendedTypeCacheTests
    {
        [Test]
        public void Should_ClearCache()
        {
            var options = TypeSupportOptions.All;
            var type = new ExtendedType(typeof(BasicObject), options);
            ExtendedTypeCache.CacheType(type, options);
            Assert.Greater(ExtendedTypeCache.Instance.CachedTypes.Count, 0);
            ExtendedTypeCache.Clear();
            Assert.AreEqual(0, ExtendedTypeCache.Instance.CachedTypes.Count);
        }

        [Test]
        public void Should_CacheTypeByAllOptions()
        {
            ExtendedTypeCache.Clear();
            var options = TypeSupportOptions.All;
            var type = new ExtendedType(typeof(BasicObject), options);
            // cache manually
            ExtendedTypeCache.CacheType(type, options);
            var extendedType = ExtendedTypeCache.Get(typeof(BasicObject), options);
            Assert.NotNull(extendedType);
            Assert.AreEqual(extendedType.Type, typeof(BasicObject));
        }

        [Test]
        public void Should_CacheTypeByFieldOptions()
        {
            ExtendedTypeCache.Clear();
            var fieldOptions = TypeSupportOptions.Fields;
            var typeWithFields = new ExtendedType(typeof(BasicObject), fieldOptions);
            // cache manually
            ExtendedTypeCache.CacheType(typeWithFields, fieldOptions);

            var propertyOptions = TypeSupportOptions.Properties;
            var typeWithProperties = new ExtendedType(typeof(BasicObject), propertyOptions);
            // cache manually
            ExtendedTypeCache.CacheType(typeWithProperties, propertyOptions);

            var extendedTypeWithFields = ExtendedTypeCache.Get(typeof(BasicObject), fieldOptions);
            var extendedTypeWithProperties = ExtendedTypeCache.Get(typeof(BasicObject), propertyOptions);

            Assert.NotNull(extendedTypeWithFields);
            Assert.NotNull(extendedTypeWithProperties);
            Assert.AreNotEqual(extendedTypeWithFields, extendedTypeWithProperties);
        }

        [Test]
        public void ShouldNot_CacheTypeByDifferentOptions()
        {
            ExtendedTypeCache.Clear();
            var options = TypeSupportOptions.Properties;
            var type = new ExtendedType(typeof(BasicObject), options);
            // cache manually
            ExtendedTypeCache.CacheType(type, options);
            Assert.Throws<InvalidOperationException>(() => { ExtendedTypeCache.Get(typeof(BasicObject), TypeSupportOptions.All); });
        }

        [Test]
        public void Should_UpgradeCacheTypeWhenAllRequested()
        {
            ExtendedTypeCache.Clear();
            var options = TypeSupportOptions.Properties;
            var type = new ExtendedType(typeof(BasicObject), options);
            // cache manually
            ExtendedTypeCache.CacheType(type, options);
            var allType = new ExtendedType(typeof(BasicObject), TypeSupportOptions.All);
            var extendedType = ExtendedTypeCache.Get(typeof(BasicObject), TypeSupportOptions.All);
            Assert.NotNull(extendedType);
            Assert.AreEqual(extendedType.Type, typeof(BasicObject));
        }

        [Test]
        public void ShouldNot_CacheTypeWithoutCacheOption()
        {
            ExtendedTypeCache.Clear();
            // don't supply Caching option
            var options = TypeSupportOptions.Properties;
            var type = new ExtendedType(typeof(BasicObject), options);
            Assert.Throws<InvalidOperationException>(() => { ExtendedTypeCache.Get(typeof(BasicObject), options); });
        }

        [Test]
        public void Should_CacheTypeWithOption()
        {
            ExtendedTypeCache.Clear();
            // supply caching option
            var options = TypeSupportOptions.Properties | TypeSupportOptions.Caching;
            var type = new ExtendedType(typeof(BasicObject), options);
            var extendedType = ExtendedTypeCache.Get(typeof(BasicObject), options);
            Assert.NotNull(extendedType);
        }
    }
}
