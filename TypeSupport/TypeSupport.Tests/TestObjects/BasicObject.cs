#pragma warning disable 0649,0169
namespace TypeSupport.Tests.TestObjects
{
    public class BasicObject
    {
        private int _test;
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
        }
    }
}
