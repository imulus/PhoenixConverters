using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixConverters.Extentions
{
    public static class Extensions
    {
        public static string TruncateAtWord(this string text, int maxCharacters, string trailingStringIfTextCut = "…")
        {
            if (text == null || (text = text.Trim()).Length <= maxCharacters)
                return text;

            var trailLength = trailingStringIfTextCut.StartsWith("&") ? 1 : trailingStringIfTextCut.Length;
            maxCharacters = maxCharacters - trailLength >= 0 ? maxCharacters - trailLength : 0;
            var pos = text.LastIndexOf(" ", maxCharacters);
            if (pos >= 0)
                return text.Substring(0, pos) + trailingStringIfTextCut;

            return string.Empty;
        }
    }
}
