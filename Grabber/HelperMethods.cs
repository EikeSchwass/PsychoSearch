using System;
using System.Linq;

namespace Grabber
{
    public static class HelperMethods
    {
        public static string[] SplitByNewLine(this string source)
        {
            return source.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        }
    }
}