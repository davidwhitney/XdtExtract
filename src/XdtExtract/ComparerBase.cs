using System.Collections.Generic;
using System.Xml.Linq;

namespace XdtExtract
{
    public abstract class ComparerBase : ICompareAppConfigs
    {
        public IEnumerable<Diff> Compare(string @base, string comparison)
        {
            return Compare(XDocument.Parse(@base), XDocument.Parse(comparison));
        }

        public abstract IEnumerable<Diff> Compare(XDocument @base, XDocument comparison);
    }
}