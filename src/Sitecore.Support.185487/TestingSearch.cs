using Sitecore.ContentTesting.ContentSearch;

namespace Sitecore.Support.ContentTesting.ContentSearch
{
    using Diagnostics;
    using Sitecore;
    using Sitecore.ContentSearch;
    using Sitecore.ContentSearch.Linq.Utilities;
    using Sitecore.ContentSearch.Security;
    using Sitecore.ContentTesting.ContentSearch.Models;
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public class TestingSearch : Sitecore.ContentTesting.ContentSearch.TestingSearch
    {
        public new TestMetricsRange GetSuggestedTestMetricsRange()
        {
            TestMetricsRange range = new TestMetricsRange();
            ISearchIndex suggestedTestSearchIndex = GetSuggestedTestSearchIndex();
            if (suggestedTestSearchIndex != null)
            {
                using (IProviderSearchContext context = suggestedTestSearchIndex.CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck))
                {
                    Expression<Func<SuggestedTestSearchResultItem, bool>> first = PredicateBuilder.True<SuggestedTestSearchResultItem>();
                    first = first.And<SuggestedTestSearchResultItem>(x => x.IsLatest);
                    first = first.And<SuggestedTestSearchResultItem>(x => x.Language == Context.Language.Name);
                    first = first.And<SuggestedTestSearchResultItem>(x => x.RawSuggestions != string.Empty);
                    IQueryable<SuggestedTestSearchResultItem> queryable = context.GetQueryable<SuggestedTestSearchResultItem>().Where<SuggestedTestSearchResultItem>(first);
                    range.ImpactMax = double.MinValue;
                    range.ImpactMin = double.MaxValue;
                    range.RecommendationMax = double.MinValue;
                    range.RecommendationMin = double.MaxValue;
                    range.PotentialMax = double.MinValue;
                    range.PotentialMin = double.MaxValue;
                    foreach (SuggestedTestSearchResultItem item in queryable)
                    {
                        if (item.Impact > range.ImpactMax) range.ImpactMax = item.Impact;
                        if (item.Recommendation > range.RecommendationMax) range.RecommendationMax = item.Recommendation;
                        if (item.Potential > range.PotentialMax) range.PotentialMax = item.Potential;
                        if (item.Impact < range.ImpactMin) range.ImpactMin = item.Impact;
                        if (item.Recommendation < range.RecommendationMin) range.RecommendationMin = item.Recommendation;
                        if (item.Potential < range.PotentialMin) range.PotentialMin = item.Potential;
                    }
                }
            }
            return range;
        }
    }
}
