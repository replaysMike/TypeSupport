using NUnit.Framework;
using System;
using TypeSupport.Tests.TestObjects;
using TypeSupport.Extensions;

namespace TypeSupport.Tests.Assembly
{
    [TestFixture]
    [NonParallelizable]
    public class ExtendedTypeCacheTests
    {
        [Test]
        public void Should_ClearCache()
        {
            var options = TypeSupportOptions.All;
            var type = typeof(BasicObject).GetExtendedType(options);
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
            var type = typeof(BasicObject).GetExtendedType(options);
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
            var fieldOptions = TypeSupportOptions.Fields | TypeSupportOptions.Caching;
            var typeWithFields = typeof(BasicObject).GetExtendedType(fieldOptions);

            var propertyOptions = TypeSupportOptions.Properties | TypeSupportOptions.Caching;
            var typeWithProperties = typeof(BasicObject).GetExtendedType(propertyOptions);

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
            var type = typeof(BasicObject).GetExtendedType(options);
            // cache manually
            ExtendedTypeCache.CacheType(type, options);
            var cacheResult =  ExtendedTypeCache.Get(typeof(BasicObject), TypeSupportOptions.All);
            Assert.IsNull(cacheResult);
        }

        [Test]
        public void Should_UpgradeCacheTypeWhenAllRequested()
        {
            ExtendedTypeCache.Clear();
            var options = TypeSupportOptions.Properties;
            var type = typeof(BasicObject).GetExtendedType(options);
            // cache manually
            ExtendedTypeCache.CacheType(type, options);
            _ = typeof(BasicObject).GetExtendedType(TypeSupportOptions.All);
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
            var type = typeof(BasicObject).GetExtendedType(options);
            var cacheResult = ExtendedTypeCache.Get(typeof(BasicObject), options);
            Assert.IsNull(cacheResult);
        }

        [Test]
        public void Should_CacheTypeWithOption()
        {
            ExtendedTypeCache.Clear();
            // supply caching option
            var options = TypeSupportOptions.Properties | TypeSupportOptions.Caching;
            var type = typeof(BasicObject).GetExtendedType(options);
            var extendedType = ExtendedTypeCache.Get(typeof(BasicObject), options);
            Assert.NotNull(extendedType);
        }

        [Test]
        public void Should_CacheTypeWithoutDuplication()
        {
            ExtendedTypeCache.Clear();
            // supply caching option
            var options = TypeSupportOptions.Properties | TypeSupportOptions.Caching;
            var type = typeof(BasicObject).GetExtendedType(options);
            for (var i = 0; i < 50; i++)
            {
                var extendedType = ExtendedTypeCache.Get(typeof(BasicObject), options);
                Assert.NotNull(extendedType);
            }
            Assert.AreEqual(1, ExtendedTypeCache.Instance.CachedTypes.Count);
        }
    }
}
