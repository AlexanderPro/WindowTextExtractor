using System;

namespace WindowTextExtractor.Extensions
{
    public static class StringExtensions
    {
        public static string TrimEnd(this string text, string value, StringComparison comparisonType = StringComparison.CurrentCulture)
        {
            var result = text;
            while (result.EndsWith(value, comparisonType))
            {
                result = result.Substring(0, result.Length - value.Length);
            }
            return result;
        }
    }
}
