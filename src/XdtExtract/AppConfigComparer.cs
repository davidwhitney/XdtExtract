using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace XdtExtract
{
    public class IterateOver
    {
        public static IEnumerable<ComparisonPair<XElement>> Pairs(XDocument baseDoc, XDocument comparisonDoc)
        {
            var baseKeys = new List<string>();

            foreach (var appSetting in baseDoc.AppSettings())
            {
                var sourceSettingKey = appSetting.Attributes().Key();
                var comparisonSetting = comparisonDoc.AppSettings().SettingOrDefault(sourceSettingKey);

                baseKeys.Add(sourceSettingKey);

                yield return new ComparisonPair<XElement>(appSetting, comparisonSetting);
            }

            foreach (var otherSetting in comparisonDoc.AppSettings())
            {
                var key = otherSetting.Attributes().Key();

                if (!baseKeys.Contains(key))
                {
                   yield return new ComparisonPair<XElement>(null, otherSetting); 
                }
            }
        }
    }

    public class ComparisonPair<T>
    {
        public T BaseItem { get; set; }
        public T ComparisonItem { get; set; }

        public ComparisonPair(T baseItem, T comparisonItem)
        {
            BaseItem = baseItem;
            ComparisonItem = comparisonItem;
        }
    }

    public class AppConfigComparer
    {
        public IEnumerable<Diff> Compare(string baseConfig, string comparisonConfig)
        {
            var baseDoc = XDocument.Parse(baseConfig);
            var comparisonDoc = XDocument.Parse(comparisonConfig); 
            
            var diffs = new List<Diff>();

            foreach (var settingPair in IterateOver.Pairs(baseDoc, comparisonDoc))
            {
                if (settingPair.ComparisonItem == null)
                {
                    diffs.Add(new Diff
                    {
                        Operation = Operation.Remove,
                        XPath = "/configuration/appSettings/add[@key=\""+settingPair.BaseItem+"\"]"
                    });
                }

                foreach (var attribute in settingPair.BaseItem.Attributes())
                {
                    
                }

                Debug.WriteLine(settingPair.BaseItem.Name);
            }

            return diffs;
        }
    }

    public static class XDocumentExtensions
    {
        public static IEnumerable<XElement> AppSettings(this XDocument src)
        {
            return src.Descendants().Where(x => x.Name == "appSettings").Descendants();
        }

        public static XElement SettingOrDefault(this IEnumerable<XElement> src, string key)
        {
            return src.SingleOrDefault(x => x.Attributes().Single(attr => attr.Name == "key").Value == key);
        } 

        public static string Key(this IEnumerable<XAttribute> src)
        {
            return src.Single(x => x.Name == "key").Value;
        } 
    }

    public class Diff
    {
        public string XPath { get; set; }
        public Operation Operation { get; set; }
    }

    public enum Operation
    {
        Add,
        Remove,
        Modify
    }
}