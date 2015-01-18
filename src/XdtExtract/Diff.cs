using System.Collections.Generic;
using System.Xml.Linq;

namespace XdtExtract
{
    public class Diff
    {
        public string FullName { get; set; }
        public Operation Operation { get; set; }

        public string Key { get; set; }
        public XElement FinalValue { get; set; }
    }
}