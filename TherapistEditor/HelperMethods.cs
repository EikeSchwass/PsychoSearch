using System;
using System.Linq;
using HtmlAgilityPack;

namespace TherapistEditor
{
    public static class HelperMethods
    {
        public static string[] SplitByNewLine(this string source)
        {
            return source.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        }

        public static string GetDecodedInnerText(this HtmlNode htmlNode) => HtmlEntity.DeEntitize(htmlNode.InnerText);

        public static string Simplify(this string s)
        {
            s = s.Replace("\n", "");
            s = s.Replace("\t", "");
            s = s.Replace("\r", "");
            s = s.Trim();
            return s;
        }

        public static bool HasInnerText(this HtmlNode htmlNode)
        {
            return !string.IsNullOrWhiteSpace(htmlNode.GetDecodedInnerText());
        }
    }
}