#pragma warning disable 0649,0169
namespace TypeSupport.Tests.TestObjects
{
    public class InheritedObject : BaseInheritedObject
    {
        private readonly string _name;
        public int Id { get; set; }
    }

    public class BaseInheritedObject
    {
        private readonly string _baseName;
        public int BaseId { get; set; }
    }
}
