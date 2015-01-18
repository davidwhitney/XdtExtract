using System;
using System.Linq;
using System.Xml.Linq;

namespace XdtExtract
{
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
                var summaryOfJustThisNode = Xel.Descendants().Aggregate(InnerText, (current, child) => current.Replace(child.JustThisNodeText(), ""));
                return String.Join(":", FullName, summaryOfJustThisNode);
            }
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
            return String.Equals(ComparisonKey, other.ComparisonKey);
        }

        public override int GetHashCode()
        {
            return (ComparisonKey != null ? ComparisonKey.GetHashCode() : 0);
        }

        public override string ToString()
        {
            // For debugging
            return ComparisonKey;
        }
    }
}