using System.Collections.Generic;
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
                var fullName = string.Join("", GenerateNamespace(node), node.Name.LocalName);
                index.Add(new IndexedXElement(fullName, node));
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
    }
}