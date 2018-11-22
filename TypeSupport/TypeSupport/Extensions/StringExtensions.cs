using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TypeSupport.Extensions
{
    /// <summary>
    /// String extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
	    /// Removes all whitespace in a string (leading, trailing, inside)
	    /// </summary>
	    /// <param name="str"></param>
	    /// <returns></returns>
	    public static string RemoveWhiteSpace(this string str)
        {
            return new string(str.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }

        /// <summary>
        /// Expand a string into multiple words based on casing
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ExpandOnCase(this string str)
        {
            return Regex.Replace(str, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled);
        }

        /// <summary>
        /// Contains extension with case sensitivity options
        /// </summary>
        /// <param name="source"></param>
        /// <param name="toCheck"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}
