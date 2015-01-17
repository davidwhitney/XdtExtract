using System;
using System.Linq;
using NUnit.Framework;

namespace XdtExtract.Test.Unit
{
    [TestFixture]
    public class AppSettingsComparerTests
    {
        private AppSettingsComparer _comparer;

        [SetUp]
        public void SetUp()
        {
            _comparer = new AppSettingsComparer();
        }

        [Test]
        public void DetectChanges_WithAppConfigContainingSingleAppSettingThatGetsRemoved_ReturnsChange()
        {
            var @base = ConfigWithSettings(@"<add key=""Value"" value=""true"" />");
            var comparison = ConfigWithSettings();
            
            var diffs = _comparer.Compare(@base, comparison).ToList();

            Assert.That(diffs[0].XPath, Is.EqualTo(@"/configuration/appSettings/add[@key='Value']"));
            Assert.That(diffs[0].Type, Is.EqualTo(Operation.Remove));
        }

        [Test]
        public void DetectChanges_WithDestinationAppSettingContainsSingleAddition_ReturnsChange()
        {
            var @base = ConfigWithSettings();
            var comparison = ConfigWithSettings(@"<add key=""Value"" value=""true"" />");
            
            var diffs = _comparer.Compare(@base, comparison).ToList();

            Assert.That(diffs[0].XPath, Is.EqualTo(@"/configuration/appSettings/add[@key='Value']"));
            Assert.That(diffs[0].Type, Is.EqualTo(Operation.Add));
        }

        [Test]
        public void DetectChanges_WithAppConfigContainingSingleAppSettingDifference_ReturnsChange()
        {
            var @base = ConfigWithSettings(@"<add key=""Value"" value=""true"" />");
            var comparison = ConfigWithSettings(@"<add key=""Value"" value=""false"" />");
            
            var diffs = _comparer.Compare(@base, comparison).ToList();

            Assert.That(diffs[0].XPath, Is.EqualTo(@"/configuration/appSettings/add[@key='Value']"));
            Assert.That(diffs[0].Type, Is.EqualTo(Operation.Modify));
            Assert.That(diffs[0].DifferenceType, Is.EqualTo(DifferenceType.Attribute));
            Assert.That(diffs[0].Key, Is.EqualTo("value"));
            Assert.That(diffs[0].NewValue, Is.EqualTo("false"));
        }

        [Test]
        public void DetectChanges_WithAppConfigContainingMultipleAttributeDifferences_ReturnsChange()
        {
            var @base = ConfigWithSettings(@"<add key=""Value"" value=""true"" />");
            var comparison = ConfigWithSettings(@"<add key=""Value"" value=""false"" value1=""blah"" />");
            
            var diffs = _comparer.Compare(@base, comparison).ToList();

            Assert.That(diffs.Count, Is.EqualTo(2));

            Assert.That(diffs[0].XPath, Is.EqualTo(@"/configuration/appSettings/add[@key='Value']"));
            Assert.That(diffs[0].Type, Is.EqualTo(Operation.Modify));
            Assert.That(diffs[0].NewValue, Is.EqualTo("false"));

            Assert.That(diffs[1].XPath, Is.EqualTo(@"/configuration/appSettings/add[@key='Value']"));
            Assert.That(diffs[1].Type, Is.EqualTo(Operation.Add));
            Assert.That(diffs[1].NewValue, Is.EqualTo("blah"));
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
