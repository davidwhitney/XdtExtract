using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        public class IndexedXElement
        {
            public string FullName { get; set; }
            public XElement Xel { get; set; }

            public string InnerText
            {
                get { return Xel.JustThisNodeText(); }
            }
            
            public bool HasNoChildren
            {
                get { return !Xel.Descendants().Any(); }
            }

            public IndexedXElement(string fullName, XElement xel)
            {
                FullName = fullName;
                Xel = xel;
            }

            public string ComparisonKey
            {
                get
                {
                    var summaryOfJustThisNode = InnerText;

                    foreach (var child in Xel.Descendants())
                    {
                        summaryOfJustThisNode = summaryOfJustThisNode.Replace(child.JustThisNodeText(), "");
                    }


                    return string.Join(":", FullName, summaryOfJustThisNode);
                }
            }

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

    public static class XElementExtensions
    {
        public static string JustThisNodeText(this XElement xel)
        {
            var line = xel.ToString().Replace(Environment.NewLine, " ");
            return Regex.Replace(line, ">\\s*<", "><");
        }
    }
}