using System;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace XdtExtract.Test.Unit
{
    [TestFixture]
    public class XmlMapGeneratorTests
    {
        private XmlMapGenerator _gen;

        [SetUp]
        public void SetUp()
        {
            _gen = new XmlMapGenerator();
        }

        [Test]
        public void FlattenXml_GivenBasicStructure_NodeKeysAreFlattenedCorrectly()
        {
            var @base = XDocument.Parse(ConfigWithSettings(@"<add key=""First"" value=""true"" />"));
            
            var diffs = _gen.FlattenXml(@base);

            Assert.That(diffs.Count, Is.EqualTo(3));
            Assert.That(diffs[0].Key, Is.EqualTo("configuration"));
            Assert.That(diffs[1].Key, Is.EqualTo("configuration.appSettings"));
            Assert.That(diffs[2].Key, Is.EqualTo("configuration.appSettings.add"));
        }

        [Test]
        public void FlattenXml_GivenBasicStructure_BranchNodeIdentifiedInMapping()
        {
            var @base = XDocument.Parse(ConfigWithSettings(@"<add key=""First"" value=""true"" />"));
            
            var diffs = _gen.FlattenXml(@base);

            Assert.That(diffs[1].Key, Is.EqualTo("configuration.appSettings"));
            Assert.That(diffs[1].IsLeafNode, Is.EqualTo(false));
        }

        [Test]
        public void FlattenXml_GivenBasicStructure_LeafNodeIdentifiedInMapping()
        {
            var @base = XDocument.Parse(ConfigWithSettings(@"<add key=""First"" value=""true"" />"));
            
            var diffs = _gen.FlattenXml(@base);

            Assert.That(diffs[2].Key, Is.EqualTo("configuration.appSettings.add"));
            Assert.That(diffs[2].IsLeafNode, Is.EqualTo(true));
        }

        [TestCase(@"<add key=""First"" value=""true"" />
                    <add key=""Second"" value=""false"" />")]
        public void FlattenXml_GivenStructureWithDuplicateNodes_BothDupedNodesPresent(string baseContent)
        {
            var @base = XDocument.Parse(ConfigWithSettings(baseContent));
            
            var diffs = _gen.FlattenXml(@base);

            Assert.That(diffs[2].Key, Is.EqualTo("configuration.appSettings.add"));
            Assert.That(diffs[3].Key, Is.EqualTo("configuration.appSettings.add"));
        }

        private static string ConfigWithSettings(string xml = "")
        {
            return ConfigWithSettings(new[] { xml });
        }

        private static string ConfigWithSettings(string[] xml)
        {
            return @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>" + string.Join(Environment.NewLine, xml) +
                   @"  </appSettings>
</configuration>";
        }
    
    }
}
