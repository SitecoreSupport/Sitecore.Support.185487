using Sitecore.Analytics.Data.Items;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Support.ContentTesting.Helpers
{
    public static class TestDataHelper
    {
        public static void NormalizeTestData<T>(IEnumerable<T> data, Func<T, double> selector, Action<T, double> assignor, double normalizedMaximum = 100.0) where T : class
        {
            Assert.ArgumentNotNull(selector, "selector");
            Assert.ArgumentNotNull(assignor, "assignor");
            if (data != null)
            {
                double dataMin = 0.0;
                T[] source = data.ToArray<T>();
                if (source.Length != 0)
                {
                    dataMin = source.Min<T>(selector);
                    NormalizeTestData<T>(source, selector, assignor, normalizedMaximum, dataMin, source.Max<T>(selector));
                }
            }
        }

        public static void NormalizeTestData<T>(IEnumerable<T> data, Func<T, double> selector, Action<T, double> assignor, double normalizedMaximum, double dataMin, double dataMax) where T : class
        {
            Assert.ArgumentNotNull(selector, "selector");
            Assert.ArgumentNotNull(assignor, "assignor");
            if (data != null)
            {
                double num = 0.0;
                if (dataMin < 0.0)
                {
                    num = Math.Abs(dataMin);
                }
                foreach (T local in data)
                {
                    double num2 = selector(local);
                    assignor(local, (dataMax <= 0.0) ? 0.0 : (((num2 + num) / (dataMax + num)) * normalizedMaximum));
                }
            }
        }

        public static TestTypeItem ResolveTestType(TemplateID testVariableTemplateId)
        {
            TemplateItem item = Context.ContentDatabase.Templates[testVariableTemplateId.ID];
            if (item == null)
            {
                return null;
            }
            Item standardValues = item.StandardValues;
            if (standardValues == null)
            {
                return null;
            }
            string str = standardValues["Test Type"];
            if (string.IsNullOrEmpty(str) && (standardValues.Language.Name != "en"))
            {
                str = Context.ContentDatabase.GetItem(standardValues.ID, Language.Parse("en"))["Test Type"];
                if (string.IsNullOrEmpty(str))
                {
                    return null;
                }
            }
            return new TestTypeItem(Context.ContentDatabase.GetItem(str));
        }
    }
}

