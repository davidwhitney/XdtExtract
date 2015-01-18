using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XdtExtract
{
    public static class XDocumentExtensions
    {
        public static IEnumerable<XElement> AppSettings(this XDocument src)
        {
            return src.Descendants().Where(x => x.Name == "appSettings").Descendants();
        }

        public static XElement SettingOrDefault(this IEnumerable<XElement> src, string key)
        {
            return src.SingleOrDefault(x => x.Attributes().Single(attr => attr.Name == "key").Value == key);
        } 

        public static string Key(this IEnumerable<XAttribute> src)
        {
            var keyAttrib = src.SingleOrDefault(x => x.Name == "key");
            return keyAttrib == null ? "" : keyAttrib.Value;
        } 
    }
}