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
        public void DetectChanges_DocumentIsIdentical_ReturnsNoChanges()
        {
            var diffs = _comparer.Compare(ConfigWithSettings(), ConfigWithSettings());

            Assert.That(diffs, Is.Empty);
        }

        [TestCase(@"<add key=""Value"" value=""true"" />", "")]
        [TestCase("", @"<add key=""Value"" value=""true"" />")]
        [TestCase(@"<add key=""Value"" value=""true"" />", @"<add key=""Value"" value=""false"" />")]
        [TestCase(@"<add key=""Value"" />", @"<add key=""Value"" value=""false"" />")]
        [TestCase(@"<add key=""Value""  value=""false""/>", @"<add key=""Value"" />")]
        public void DetectChanges_WithChangesOfMultipleTypes_DiffHighlightsCorrectXPath(string baseContent, string comparisonContent)
        {
            var @base = ConfigWithSettings(baseContent);
            var comparison = ConfigWithSettings(comparisonContent);
            
            var diffs = _comparer.Compare(@base, comparison).ToList();

            Assert.That(diffs[0].FullName, Is.EqualTo(@"configuration.appSettings.add"));
        }

        [TestCase(@"<add key=""Value"" value=""true"" />", "")]
        [TestCase("", @"<add key=""Value"" value=""true"" />")]
        [TestCase(@"<add key=""Value"" value=""true"" />", @"<add key=""Value"" value=""false"" />")]
        [TestCase(@"<add key=""Value"" />", @"<add key=""Value"" value=""false"" />")]
        [TestCase(@"<add key=""Value""  value=""false""/>", @"<add key=""Value"" />")]
        public void DetectChanges_WithChangesOfMultipleTypes_DiffContainsTheCorrectCountOfChanges(string baseContent, string comparisonContent)
        {
            var @base = ConfigWithSettings(baseContent);
            var comparison = ConfigWithSettings(comparisonContent);
            
            var diffs = _comparer.Compare(@base, comparison).ToList();

            Assert.That(diffs.Count(), Is.EqualTo(1));
        }

        [Test]
        public void DetectChanges_WithAppConfigContainingSingleAppSettingThatGetsRemoved_ReturnsChange()
        {
            var @base = ConfigWithSettings(@"<add key=""Value"" value=""true"" />");
            var comparison = ConfigWithSettings();
            
            var diffs = _comparer.Compare(@base, comparison).ToList();

            Assert.That(diffs[0].Operation, Is.EqualTo(Operation.Remove));
        }

        [Test]
        public void DetectChanges_WithDestinationAppSettingContainsSingleAddition_ReturnsChange()
        {
            var @base = ConfigWithSettings();
            var comparison = ConfigWithSettings(@"<add key=""Value"" value=""true"" />");
            
            var diffs = _comparer.Compare(@base, comparison).ToList();

            Assert.That(diffs[0].Operation, Is.EqualTo(Operation.Add));
        }

        [Test]
        public void DetectChanges_WithAppConfigContainingSingleAppSettingDifference_ReturnsChange()
        {
            var @base = ConfigWithSettings(@"<add key=""First"" value=""true"" />");
            var comparison = ConfigWithSettings(@"<add key=""First"" value=""false"" />");
            
            var diffs = _comparer.Compare(@base, comparison).ToList();

            Assert.That(diffs[0].Operation, Is.EqualTo(Operation.Modify));
            Assert.That(diffs[0].Key, Is.EqualTo("First"));
            Assert.That(diffs[0].FinalValue.ToString(), Is.StringContaining("value=\"false\""));
        }

        [Test]
        public void DetectChanges_WithAppConfigContainingMultipleAttributeDifferences_ReturnsChange()
        {
            var @base = ConfigWithSettings(@"<add key=""First"" value=""true"" />");
            var comparison = ConfigWithSettings(@"<add key=""First"" value=""false"" value1=""blah"" />");
            
            var diffs = _comparer.Compare(@base, comparison).ToList();

            Assert.That(diffs.Count, Is.EqualTo(2));

            Assert.That(diffs[0].Operation, Is.EqualTo(Operation.Modify));
            Assert.That(diffs[0].FinalValue, Is.EqualTo("false"));

            Assert.That(diffs[1].Operation, Is.EqualTo(Operation.Add));
            Assert.That(diffs[1].FinalValue, Is.EqualTo("blah"));
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
