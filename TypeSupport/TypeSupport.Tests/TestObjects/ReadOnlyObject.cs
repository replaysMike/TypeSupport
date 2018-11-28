using System.Collections;
using System.Collections.Generic;

namespace TypeSupport.Tests.TestObjects
{
    public class ReadOnlyObject
    {
        private readonly Dictionary<string, IList> inner = new Dictionary<string, IList>();
    }
}
