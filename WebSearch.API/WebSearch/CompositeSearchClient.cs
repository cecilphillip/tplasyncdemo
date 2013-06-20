using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSearch.API.WebSearch
{
    public class CompositeSearchClient : ISearchEngine
    {
        protected IEnumerable<ISearchEngine> SearchEngines { get; set; }

        public CompositeSearchClient(IEnumerable<ISearchEngine> searchEngines)
        {
            SearchEngines = searchEngines;
        }

        public string Name
        {
            get
            {
                return string.Join(",", SearchEngines.Select(engine => engine.Name));
            }
        }

        public Task<IEnumerable<SearchResult>> WebSearch(string searchTerm)
        {
            var taskList = new List<Task<IEnumerable<SearchResult>>>();
            foreach (var engine in SearchEngines)
            {
                var webSearch = engine.WebSearch(searchTerm);
                taskList.Add(webSearch);
            }

            return Task.Factory.ContinueWhenAll(taskList.ToArray(), tasks =>
                   {
                       var searchResults = new List<SearchResult>();
                       foreach (Task<IEnumerable<SearchResult>> task in tasks)
                       {
                           if ( task.Exception == null && !task.IsFaulted)
                           {
                               searchResults.AddRange(task.Result);
                           }
                       }
                       return searchResults.AsEnumerable();
                   });

        }
    }
}