using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace XdtExtract
{
    public static class XElementExtensions
    {
        public static string JustThisNodeText(this XElement xel)
        {
            var line = xel.ToString().Replace(Environment.NewLine, " ");
            return Regex.Replace(line, ">\\s*<", "><");
        }
    }
}