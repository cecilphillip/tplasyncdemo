using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bing;

namespace WebSearch.API.WebSearch
{
    public class BingSearchClient : ISearchEngine
    {
        protected BingSearchContainer searchContainer;
        protected const string BING_SERVICE_ROOT = "https://api.datamarket.azure.com/Bing/Search/v1/";
        private const string accountKey = "lHn1H6sN95avNx0zuS+ckOMKfptz3eQw9EM/UpyCnWI";

        public BingSearchClient(): this(accountKey){ }
        public BingSearchClient(string accountKey)
        {
            this.searchContainer = new BingSearchContainer(new Uri(BING_SERVICE_ROOT))
                {
                    Credentials = new NetworkCredential(accountKey, accountKey)
                };
        }

        public string Name { get { return "BING"; } }

        public virtual Task<IEnumerable<SearchResult>> WebSearch(string searchTerm)
        {            
            var query = searchContainer.Web(searchTerm, null, null, null, null, null, null, null);
            query = query.AddQueryOption("$top", SearchConstants.SEARCH_COUNT);
            
            return Task.Factory.FromAsync<IEnumerable<WebResult>>(query.BeginExecute, query.EndExecute, query)
                   .ContinueWith(task =>
                   {
                       Thread.Sleep(5000);
                       if (task.Exception != null) throw task.Exception;

                       var results = task.Result.Select(result => new SearchResult()
                            {
                                Title = result.Title,
                                Description = result.Description,
                                Url = result.Url
                            });
                       return results;
                   });
        }
    }
}