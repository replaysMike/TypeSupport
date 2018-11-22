using System;
using System.Collections.Concurrent;

namespace TypeSupport.Assembly
{
    /// <summary>
	/// Cache for storing dynamic types
	/// </summary>
	public static class DynamicTypeCache
    {
        private static ConcurrentDictionary<string, Type> Types = new ConcurrentDictionary<string, Type>();

        public static Type GetOrAdd(string key, Func<string, Type> valueFactory)
        {
            return Types.GetOrAdd(key, valueFactory);
        }

        public static bool TryGetValue(string key, out Type value)
        {
            return Types.TryGetValue(key, out value);
        }
    }
}
