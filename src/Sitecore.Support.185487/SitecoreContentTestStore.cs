namespace Sitecore.Support.ContentTesting.Data
{
    using Sitecore.ContentTesting.ContentSearch;
    using Sitecore.ContentTesting.ContentSearch.Models;
    using Sitecore.ContentTesting.Helpers;
    using Sitecore.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SitecoreContentTestStore : Sitecore.ContentTesting.Data.SitecoreContentTestStore
    {
        public override IEnumerable<SuggestedTestSearchResultItem> GetSuggestedTests(DataUri hostItemDataUri, string searchText = null)
        {
            Sitecore.Support.ContentTesting.ContentSearch.TestingSearch search = new Sitecore.Support.ContentTesting.ContentSearch.TestingSearch();
            if (hostItemDataUri != null)
            {
                search.HostItem = hostItemDataUri;
            }
            if (!string.IsNullOrEmpty(searchText))
            {
                search.SearchText = searchText;
            }
            SuggestedTestSearchResultItem[] data = search.GetSuggestedTests().ToArray<SuggestedTestSearchResultItem>();
            TestMetricsRange suggestedTestMetricsRange = search.GetSuggestedTestMetricsRange();
            TestDataHelper.NormalizeTestData<SuggestedTestSearchResultItem>(data, r => r.Potential, delegate (SuggestedTestSearchResultItem r, double v)
            {
                r.Potential = Math.Ceiling(v);
            }, 10.0, suggestedTestMetricsRange.PotentialMin, suggestedTestMetricsRange.PotentialMax);
            TestDataHelper.NormalizeTestData<SuggestedTestSearchResultItem>(data, r => r.Impact, delegate (SuggestedTestSearchResultItem r, double v)
            {
                r.Impact = Math.Ceiling(v);
            }, 10.0, suggestedTestMetricsRange.ImpactMin, suggestedTestMetricsRange.ImpactMax);
            TestDataHelper.NormalizeTestData<SuggestedTestSearchResultItem>(data, r => r.Recommendation, delegate (SuggestedTestSearchResultItem r, double v)
            {
                r.Recommendation = Math.Ceiling(v);
            }, 10.0, suggestedTestMetricsRange.RecommendationMin, suggestedTestMetricsRange.RecommendationMax);
            data = (from x in data
                    where ((x.Potential > 0.0) && (x.Impact > 0.0)) && (x.Recommendation > 0.0)
                    select x).ToArray<SuggestedTestSearchResultItem>();
            IEnumerable<TestingSearchResultItem> source = this.GetActiveTests(hostItemDataUri, searchText, null);
            IEnumerable<ID> first = from x in data select x.ItemId;
            IEnumerable<ID> second = source.Select<TestingSearchResultItem, ID>(delegate (TestingSearchResultItem x)
            {
                if (x.HostItemUri == null)
                {
                    return null;
                }
                return x.HostItemUri.ItemID;
            });
            IEnumerable<ID> validIds = first.Except<ID>(second);
            return (from s in data
                    where validIds.Contains<ID>(s.ItemId)
                    select s);
        }
    }
}
