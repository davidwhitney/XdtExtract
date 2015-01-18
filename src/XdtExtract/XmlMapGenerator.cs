using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace XdtExtract
{
    public class XmlMapGenerator
    {
        public List<IndexedXElement> FlattenXml(XDocument @base)
        {
            var index = new List<IndexedXElement>();
            foreach (var node in @base.Descendants())
            {
                var key = string.Join("", GenerateNamespace(node), node.Name.LocalName);
                index.Add(new IndexedXElement(key, node));
            }
            return index;
        }

        private object GenerateNamespace(XElement node, string stub = "")
        {
            if (node.Parent == null)
            {
                return stub;
            }

            stub = string.Join(".", node.Parent.Name, stub);
            var ns = GenerateNamespace(node.Parent, stub);

            return ns;
        }

        public class IndexedXElement
        {
            public string Key { get; set; }
            public XElement Xel { get; set; }

            public string InnerText
            {
                get { return Xel.ToString(); }
            }

            public bool IsLeafNode
            {
                get { return !Xel.Descendants().Any(); }
            }

            public IndexedXElement(string key, XElement xel)
            {
                Key = key;
                Xel = xel;
            }

            public string ComparisonKey { get { return string.Join(":", Key, InnerText); } }

            public override string ToString()
            {
                return ComparisonKey;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((IndexedXElement) obj);
            }

            protected bool Equals(IndexedXElement other)
            {
                return string.Equals(ComparisonKey, other.ComparisonKey);
            }

            public override int GetHashCode()
            {
                return (ComparisonKey != null ? ComparisonKey.GetHashCode() : 0);
            }
        }

    }
}