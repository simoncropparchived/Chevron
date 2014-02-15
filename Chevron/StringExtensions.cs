using Scalpel;

namespace Chevron
{
    [Remove]
    static class StringExtensions
    {
        public static string FixNewLines(this string target)
        {
            return target.Replace("\r\n", "\n");
        }

    }
}