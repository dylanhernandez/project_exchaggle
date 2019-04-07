using System.Collections.Generic;

namespace Exchaggle.ViewModels.Search
{
    public class SearchResultsViewModel
    {
        public SearchResultsViewModel()
        {
            Page = 0;
        }
        public string SearchQuery { get; set; }
        public int SearchCategory { get; set; }
        public int SearchSubcategory { get; set; }
        public int TradeCategory { get; set; }
        public int TradeSubcategory { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public int Page { get; set; }
        public IEnumerable<SearchResultItemViewModel> Results { get; set; }
    }
}