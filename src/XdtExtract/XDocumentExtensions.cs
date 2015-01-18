using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XdtExtract
{
    public static class XDocumentExtensions
    {
        public static string Key(this IEnumerable<XAttribute> src)
        {
            var keyAttrib = src.SingleOrDefault(x => x.Name == "key");
            return keyAttrib != null ? keyAttrib.Value : null;
        }
    }
}