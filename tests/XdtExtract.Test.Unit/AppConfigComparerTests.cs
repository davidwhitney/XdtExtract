using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var c1 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>
    <add key=""Value"" value=""true"" />
  </appSettings>
</configuration>";

            var c2 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>
  </appSettings>
</configuration>";
            
            var diffs = _comparer.Compare(c1, c2).ToList();

            Assert.That(diffs[0].XPath, Is.EqualTo(@"/configuration/appSettings/add[@key='Value']"));
            Assert.That(diffs[0].Operation, Is.EqualTo(Operation.Remove));
        }

        [Test]
        public void DetectChanges_WithDestinationAppSettingContainsSingleAddition_ReturnsChange()
        {
            var c1 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>
  </appSettings>
</configuration>";

            var c2 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>
    <add key=""Value"" value=""true"" />
  </appSettings>
</configuration>";
            
            var diffs = _comparer.Compare(c1, c2).ToList();

            Assert.That(diffs[0].XPath, Is.EqualTo(@"/configuration/appSettings/add[@key='Value']"));
            Assert.That(diffs[0].Operation, Is.EqualTo(Operation.Add));
        }

        [Test]
        public void DetectChanges_WithAppConfigContainingSingleAppSettingDifference_ReturnsChange()
        {
            var c1 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>
    <add key=""Value"" value=""true"" />
  </appSettings>
</configuration>";

            var c2 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>
    <add key=""Value"" value=""false"" />
  </appSettings>
</configuration>";
            
            var diffs = _comparer.Compare(c1, c2).ToList();

            Assert.That(diffs[0].XPath, Is.EqualTo(@"/configuration/appSettings/add[@key='Value']"));
        }
    }
}
