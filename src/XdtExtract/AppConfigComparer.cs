using System.Collections.Generic;
using System.Xml.Linq;

namespace XdtExtract
{
    public class AppConfigComparer
    {
        private readonly List<ICompareAppConfigs> _comparers;

        public AppConfigComparer()
        {
            _comparers = new List<ICompareAppConfigs>
            {
                new AppSettingsComparer()
            };
        }

        public IEnumerable<Diff> Compare(XDocument @base, XDocument comparison)
        {
            var diffs = new List<Diff>();
            foreach (var comparer in _comparers)
            {
                diffs.AddRange(comparer.Compare(@base, comparison));
            }
            return diffs;
        }
    }
}