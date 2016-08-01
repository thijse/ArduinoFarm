using System;
using System.Text;

namespace Utilities
{
    /// <summary> String utilities. </summary>
    public class StringUtils
    {
        /// <summary> Convert string from one codepage to another. </summary>
        /// <param name="input">        The string. </param>
        /// <param name="fromEncoding"> input encoding codepage. </param>
        /// <param name="toEncoding">   output encoding codepage. </param>
        /// <returns> the encoded string. </returns>
        static public string ConvertEncoding(string input, Encoding fromEncoding, Encoding toEncoding)
        {
            var byteArray = fromEncoding.GetBytes(input);
            var asciiArray = Encoding.Convert(fromEncoding, toEncoding, byteArray);
            var finalString = toEncoding.GetString(asciiArray);
            return finalString;
        }
    }

    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }

}
