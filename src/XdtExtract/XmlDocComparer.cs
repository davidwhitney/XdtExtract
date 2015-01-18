using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XdtExtract
{
    public class XmlDocComparer
    {
        public IEnumerable<Diff> Compare(string @base, string comparison)
        {
            return Compare(XDocument.Parse(@base), XDocument.Parse(comparison));
        }

        public IEnumerable<Diff> Compare(XDocument @base, XDocument comparison)
        {
            var diffs = new List<Diff>();

            var mapper = new XmlMapGenerator();
            var baseMap = mapper.FlattenXml(@base);
            var comparisonMap = mapper.FlattenXml(comparison);

            if (MapsAreIdentical(comparisonMap, baseMap))
            {
                return new List<Diff>();
            }

            MapDeltas(comparisonMap.Except(baseMap), diffs, Operation.Add);
            MapDeltas(baseMap.Except(comparisonMap), diffs, Operation.Remove);
            MapModifications(diffs);

            return diffs;
        }

        private static void MapModifications(List<Diff> diffs)
        {
            var pairedAddAndRemoves =
                diffs.Where(x => !string.IsNullOrWhiteSpace(x.Key))
                    .GroupBy(x => x.FullName + ":" + x.Key)
                    .Where(x => x.Count() == 2);

            foreach (var pair in pairedAddAndRemoves)
            {
                if (pair.Any(x => x.Operation == Operation.Add)
                    && pair.Any(x => x.Operation == Operation.Remove))
                {
                    var removeOp = pair.Single(x => x.Operation == Operation.Remove);
                    var addOp = pair.Single(x => x.Operation == Operation.Add);

                    diffs.Remove(removeOp);

                    addOp.Operation = Operation.Modify;
                    addOp.FinalValue = addOp.FinalValue;
                }
            }
        }

        private static void MapDeltas(IEnumerable<XmlMapGenerator.IndexedXElement> except, List<Diff> diffs, Operation op)
        {
            var exceptions = except.ToList();

            foreach (var item in exceptions.Where(x => x.HasNoChildren))
            {
                var diff = new Diff
                {
                    FullName = item.FullName,
                    Key = item.Xel.Attributes().Key() ?? item.Xel.Name.LocalName,
                    Operation = op,
                    FinalValue = item.Xel
                };

                diffs.Add(diff);
            }
        }

        private static bool MapsAreIdentical(List<XmlMapGenerator.IndexedXElement> comparisonMap, List<XmlMapGenerator.IndexedXElement> baseMap)
        {
            return !comparisonMap.Except(baseMap).Any()
                   && !baseMap.Except(comparisonMap).Any();
        }
    }
}