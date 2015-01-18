using System.Collections.Generic;

namespace XdtExtract
{
    public class Diff
    {
        public string FullName { get; set; }

        public Operation Operation { get; set; }
        public DifferenceType DifferenceType { get; set; }

        public string Key { get; set; }
        public string NewValue { get; set; }
    }

    public enum DifferenceType
    {
        Value,
        Attribute
    }
}