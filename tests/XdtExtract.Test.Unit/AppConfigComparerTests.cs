using System;
using System.Linq;
using NUnit.Framework;

namespace XdtExtract.Test.Unit
{
    [TestFixture]
    public class AppConfigComparerTests
    {
        private AppConfigComparer _comparer;

        [SetUp]
        public void SetUp()
        {
            _comparer = new AppConfigComparer();
        }

        [Test]
        public void DetectChanges_WithAppConfigContainingSingleAppSettingThatGetsRemoved_ReturnsChange()
        {
            var @base = ConfigWithSettings(@"<add key=""Value"" value=""true"" />");
            var comparison = ConfigWithSettings();
            
            var diffs = _comparer.Compare(@base, comparison).ToList();

            Assert.That(diffs[0].XPath, Is.EqualTo(@"/configuration/appSettings/add[@key='Value']"));
            Assert.That(diffs[0].Operation, Is.EqualTo(Operation.Remove));
        }

        [Test]
        public void DetectChanges_WithDestinationAppSettingContainsSingleAddition_ReturnsChange()
        {
            var @base = ConfigWithSettings();
            var comparison = ConfigWithSettings(@"<add key=""Value"" value=""true"" />");
            
            var diffs = _comparer.Compare(@base, comparison).ToList();

            Assert.That(diffs[0].XPath, Is.EqualTo(@"/configuration/appSettings/add[@key='Value']"));
            Assert.That(diffs[0].Operation, Is.EqualTo(Operation.Add));
        }

        [Test]
        public void DetectChanges_WithAppConfigContainingSingleAppSettingDifference_ReturnsChange()
        {
            var @base = ConfigWithSettings(@"<add key=""Value"" value=""true"" />");
            var comparison = ConfigWithSettings(@"<add key=""Value"" value=""false"" />");
            
            var diffs = _comparer.Compare(@base, comparison).ToList();

            Assert.That(diffs[0].XPath, Is.EqualTo(@"/configuration/appSettings/add[@key='Value']"));
            Assert.That(diffs[0].Operation, Is.EqualTo(Operation.Modify));
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
