using System;
using System.Globalization;

namespace AmpHelper.Helpers {
    internal partial class HelperMethods {
        private static IFormatProvider formatProvider = CultureInfo.InvariantCulture;

        /// <summary>
        /// Parses strings to double values with InvariantCulture as the format provider.<br/>
        /// This is used to parse numbers with decimal points, without relying on the user's region preferences in Windows to determine the decimal point style.
        /// </summary>
        /// <param name="s">The string to parse from</param>
        /// <returns>A value of type double</returns>
        public static double ParseDoubleInvariant(ReadOnlySpan<char> s) => double.Parse(s, provider: formatProvider);
    }
}
