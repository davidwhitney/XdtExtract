using System.Collections.Generic;
using System.Xml.Linq;

namespace XdtExtract
{
    public interface ICompareAppConfigs
    {
        IEnumerable<Diff> Compare(XDocument @base, XDocument comparison);
    }
}