using System.ComponentModel;
using System.Runtime.Serialization;

namespace TypeSupport.Tests.TestObjects
{
    [Category("CategoryName")]
    [Description("Description value")]
    public class BasicObjectWithAttributes
    {
        [IgnoreDataMember]
        public int Id { get; set; }
    }
}
