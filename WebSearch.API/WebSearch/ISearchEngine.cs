using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebSearch.API.WebSearch
{
    public interface ISearchEngine
    {
        string Name { get; }
        Task<IEnumerable<SearchResult>> WebSearch(string searchTerm);
    }
}