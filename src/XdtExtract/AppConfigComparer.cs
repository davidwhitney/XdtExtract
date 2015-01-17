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

            foreach (var group in GroupedAppSettings(@base, comparison))
            {
                if (group.Count() == 1)
                {
                    diffs.Add(new Diff
                    {
                        XPath = "/configuration/appSettings/add[@key='" + @group.Key + "']",
                        Operation = @group.First().Source == "base" ? Operation.Remove : Operation.Add
                    });

                    continue;
                }

                foreach (var attributeGroup in GroupedAttributes(group))
                {
                    if (attributeGroup.Count() == 2)
                    {
                        var first = attributeGroup.First();
                        var second = attributeGroup.Skip(1).First();

                        if (first.Item.Value == second.Item.Value)
                        {
                            continue;
                        }

                        diffs.Add(new Diff
                        {
                            XPath = "/configuration/appSettings/add[@key='" + @group.Key + "']",
                            Operation = Operation.Modify,
                            NewValue = second.Item.Value
                        });

                        continue;
                    }
                    
                }
            }


            return diffs;
        }

        private static IEnumerable<IGrouping<string, Grouped<XElement>>> GroupedAppSettings(XDocument @base, XDocument comparison)
        {
            var baseGrp = @base.AppSettings().Select(x => new Grouped<XElement> { Source = "base", Item = x });
            var compGrp = comparison.AppSettings().Select(x => new Grouped<XElement> { Source = "comparison", Item = x });
            return baseGrp.Union(compGrp).GroupBy(x => x.Item.Attributes().Key());
        }

        private static IEnumerable<IGrouping<string, Grouped<XAttribute>>> GroupedAttributes(IGrouping<string, Grouped<XElement>> group)
        {
            return GroupedAttributes(group.First().Item, group.Skip(1).First().Item);
        }

        private static IEnumerable<IGrouping<string, Grouped<XAttribute>>> GroupedAttributes(XElement @base, XElement comparison)
        {
            var baseGrp = @base.Attributes().Select(x => new Grouped<XAttribute> { Source = "base", Item = x });
            var compGrp = comparison.Attributes().Select(x => new Grouped<XAttribute> { Source = "comparison", Item = x });
            return baseGrp.Union(compGrp).GroupBy(x => x.Item.Name.LocalName);
        }

        private class Grouped<T>
        {
            public string Source { get; set; }
            public T Item { get; set; }
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
        public string NewValue { get; set; }
    }

    public enum Operation
    {
        Add,
        Remove,
        Modify
    }
}