namespace TypeSupport.Tests.TestObjects
{
    public class InheritedObject : BaseInheritedObject
    {
        private string _name;
        public int Id { get; set; }
    }

    public class BaseInheritedObject
    {
        private string _baseName;
        public int BaseId { get; set; }
    }
}
