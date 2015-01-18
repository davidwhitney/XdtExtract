using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

namespace XdtExtract
{
    public class XmlMapGenerator
    {
        public List<KeyValuePair<string, IndexedXElement>> FlattenXml(XDocument @base)
        {
            var index = new List<KeyValuePair<string, IndexedXElement>>();
            foreach (var node in @base.Descendants())
            {
                var key = string.Join("", GenerateNamespace(node), node.Name.LocalName);
                var indexed = new IndexedXElement(node);
                index.Add(new KeyValuePair<string, IndexedXElement>(key, indexed));

                Debug.WriteLine(key);
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
            private XElement Xel { get; set; }

            public IndexedXElement(XElement xel)
            {
                Xel = xel;
            }
        }

    }
}