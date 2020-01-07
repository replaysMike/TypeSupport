#pragma warning disable 0649,0169
namespace TypeSupport.Tests.TestObjects
{
    public class BasicObject
    {
        [TestDecorated(789)]
        private readonly int _test;

        [TestDecorated(123)]
        public int Id { get; set; }

        public int Test
        {
            get
            {
                return _test;
            }
        }

        public BasicObject(int id)
        {
            Id = id;
            _test = 0;
        }

        [TestDecorated(456)]
        public bool TestMethod()
        {
            return true;
        }

        public override string ToString()
        {
            return base.ToString();
        }

    }
}
