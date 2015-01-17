using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XdtExtract
{

    public class AppConfigComparer
    {
        public IEnumerable<Diff> Compare(string @base, string comparison)
        {
            return Compare(XDocument.Parse(@base), XDocument.Parse(comparison));
        }

        public IEnumerable<Diff> Compare(XDocument @base, XDocument comparison)
        {
            var diffs = new List<Diff>();

            var baseDocSettings = @base.AppSettings().Select(x => new  { Source = "base", Node = x });
            var comparisonDocSettings = comparison.AppSettings().Select(x => new  { Source = "comparison", Node = x });
            var groups = baseDocSettings.Union(comparisonDocSettings).GroupBy(x => x.Node.Attributes().Key());

            foreach (var group in groups)
            {
                if (group.Count() == 1)
                {
                    diffs.Add(new Diff
                    {
                        XPath = "/configuration/appSettings/add[@key='" + @group.Key + "']",
                        Operation = @group.First().Source == "base" ? Operation.Remove : Operation.Add
                    });
                }
            }


            return diffs;
        }
    }

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
            return src.Single(x => x.Name == "key").Value;
        } 
    }

    public class Diff
    {
        public string XPath { get; set; }
        public Operation Operation { get; set; }
    }

    public enum Operation
    {
        Add,
        Remove,
        Modify
    }
}