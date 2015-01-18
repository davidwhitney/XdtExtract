using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XdtExtract
{
    public class AppSettingsComparer : ComparerBase
    {
    
        public override IEnumerable<Diff> Compare(XDocument @base, XDocument comparison)
        {
            var diffs = new List<Diff>();

            //var baseDictionary = FlattenXml(@base);
            //var comparisonDictionary = FlattenXml(comparison);

            var mapper = new XmlMapGenerator();
            var baseMap = mapper.FlattenXml(@base);
            var comparisonMap = mapper.FlattenXml(comparison);

            var targetMap = new List<XmlMapGenerator.IndexedXElement>(comparisonMap);
            var filteredTargetMap = targetMap.Except(baseMap).ToList();

            if (!filteredTargetMap.Any())
            {
                return new List<Diff>();
            }


            //foreach (var group in AppSettingsGroupedByKey(@base, comparison))
            //{
            //    if (group.Count() == 1)
            //    {
            //        diffs.Add(new Diff
            //        {
            //            XPath = "/configuration/appSettings/add[@key='" + @group.Key + "']",
            //            DifferenceType = DifferenceType.Value,
            //            Key = @group.Key,
            //            Type = @group.First().Source == Source.BaseFile ? Operation.Remove : Operation.Add
            //        });

            //        continue;
            //    }

            //    CompareAttributes(group.Key, GroupAttributesByKey(@group), diffs);
            //}
            return diffs;
        }


        private static void CompareAttributes(string elementKey, IEnumerable<IGrouping<string, Grouped<XAttribute>>> groupOfXmlElements, List<Diff> diffs)
        {
            foreach (var groupOfIdenticallyNamedAttributes in groupOfXmlElements)
            {
                if (groupOfIdenticallyNamedAttributes.Count() > 2)
                {
                    throw new Exception("More than 2 attributes for appSettings key " + groupOfIdenticallyNamedAttributes.Key + " exists - this is probably an error");
                }

                if (groupOfIdenticallyNamedAttributes.Count() == 1)
                {
                    ProcessAttributeOnlyPresentInOneSource(elementKey, diffs, groupOfIdenticallyNamedAttributes);
                    continue;
                }

                if (groupOfIdenticallyNamedAttributes.Count() == 2)
                {
                    if (AllAttributesHaveTheSameValue(groupOfIdenticallyNamedAttributes))
                    {
                        continue;
                    }

                    ProcessModifiedAttribute(elementKey, diffs, groupOfIdenticallyNamedAttributes.Skip(1).First());
                }
            }
        }

        private static bool AllAttributesHaveTheSameValue(IGrouping<string, Grouped<XAttribute>> groupOfIdenticallyNamedAttributes)
        {
            var first = groupOfIdenticallyNamedAttributes.First();
            var second = groupOfIdenticallyNamedAttributes.Skip(1).First();
            return first.Item.Value == second.Item.Value;
        }

        private static void ProcessModifiedAttribute(string key, List<Diff> diffs, Grouped<XAttribute> second)
        {
            diffs.Add(new Diff
            {
                XPath = "/configuration/appSettings/add[@key='" + key + "']",
                Type = Operation.Modify,
                Key = second.Item.Name.LocalName,
                NewValue = second.Item.Value,
                DifferenceType = DifferenceType.Attribute
            });
        }

        private static void ProcessAttributeOnlyPresentInOneSource(string key, List<Diff> diffs, IGrouping<string, Grouped<XAttribute>> attributeGroup)
        {
            var source = attributeGroup.First().Source;
            var itemThatIsOnlyInOneFile = attributeGroup.First().Item;

            diffs.Add(new Diff
            {
                XPath = "/configuration/appSettings/add[@key='" + key + "']",
                DifferenceType = DifferenceType.Value,
                Key = attributeGroup.Key,
                Type = source == Source.ComparisonFile ? Operation.Add : Operation.Remove,
                NewValue = itemThatIsOnlyInOneFile.Value
            });
        }

        private static IEnumerable<IGrouping<string, Grouped<XElement>>> AppSettingsGroupedByKey(XDocument @base, XDocument comparison)
        {
            var baseGrp = @base.AppSettings().Select(x => new Grouped<XElement> { Source = Source.BaseFile, Item = x });
            var compGrp = comparison.AppSettings().Select(x => new Grouped<XElement> { Source = Source.ComparisonFile, Item = x });
            return baseGrp.Union(compGrp).GroupBy(x => x.Item.Attributes().Key());
        }

        private static IEnumerable<IGrouping<string, Grouped<XAttribute>>> GroupAttributesByKey(IGrouping<string, Grouped<XElement>> group)
        {
            return GroupAttributesByKey(group.First().Item, group.Skip(1).First().Item);
        }

        private static IEnumerable<IGrouping<string, Grouped<XAttribute>>> GroupAttributesByKey(XElement @base, XElement comparison)
        {
            var baseGrp = @base.Attributes().Select(x => new Grouped<XAttribute> { Source = Source.BaseFile, Item = x });
            var compGrp = comparison.Attributes().Select(x => new Grouped<XAttribute> { Source = Source.ComparisonFile, Item = x });
            return baseGrp.Union(compGrp).GroupBy(x => x.Item.Name.LocalName);
        }

        private class Grouped<T>
        {
            public Source Source { get; set; }
            public T Item { get; set; }
        }

        private enum Source
        {
            BaseFile,
            ComparisonFile
        }
    }

}