using NUnit.Framework;
using TypeSupport.Assembly;

namespace TypeSupport.Tests.Assembly
{
    [TestFixture]
    public class DynamicAssemblyCacheTests
    {
        [Test]
        public void Should_CacheAssembly()
        {
            var assembly = DynamicAssemblyCache.Get("MyMadeUpAssembly");
            Assert.NotNull(assembly);
            Assert.AreEqual(1, DynamicAssemblyCache.Assemblies.Count);
        }
    }
}
