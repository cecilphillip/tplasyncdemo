using System.Collections.ObjectModel;
using WebSearch.API.WebSearch;

namespace WebSearch.Desktop.Model
{
    public class SearchResultsViewModel
    {
        public ObservableCollection<SearchResult> GoogleSearchResults { get; set; }
        public ObservableCollection<SearchResult> BingSearchResults { get; set; }

        public SearchResultsViewModel()
        {
            GoogleSearchResults = new ObservableCollection<SearchResult>();
            BingSearchResults = new ObservableCollection<SearchResult>();
        }
    }
}