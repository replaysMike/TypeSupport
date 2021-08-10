#pragma warning disable 0649,0169
namespace TypeSupport.Tests.TestObjects
{
    [TestDecorated(1000)]
    public class BasicObject
    {
        [TestDecorated(789)]
        private readonly int _test;

        [TestDecorated(123)]
        public int Id { get; set; }

        public int Test => _test;

        public int FieldValue;

        public BasicObject(int id)
        {
            Id = id;
            FieldValue = id;
            _test = 0;
        }

        [TestDecorated(456)]
        public bool TestMethod() => true;

        // do not remove
        public override string ToString() => base.ToString();
    }
}
