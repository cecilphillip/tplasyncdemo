using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WebSearch.API.WebSearch
{
    public class GoogleSearchClient : ISearchEngine
    {
        protected const string GoogleServiceRoot = "https://www.googleapis.com/customsearch/v1";
        protected const string ApiKey = "AIzaSyB4bPOVuuT8hD8MpIQqRHhsitEq-RB_gKY";
        protected const string CtxKey = "009417493158603247488:q-yo_uvx1fu";
        protected HttpClient client;

        public GoogleSearchClient()
        {
            this.client = HttpClientFactory.Create();
            this.client.BaseAddress = new Uri(GoogleServiceRoot);
        }

        public string Name { get { return "GOOGLE"; } }

        public virtual Task<IEnumerable<SearchResult>> WebSearch(string searchTerm)
        {
            string requestUrl = PrepareRequestUrl(searchTerm);
            return client.GetStringAsync(requestUrl).ContinueWith(task =>
                {
                    if (task.Exception != null) throw task.Exception;
                    var results = ParseResults(task.Result);
                    return results;
                });
        }

        protected virtual IEnumerable<SearchResult> ParseResults(string result)
        {
            var jsonResults = JObject.Parse(result);
            var results = jsonResults["items"].Children().Select(r => new SearchResult
            {
                Title = (string)r["title"],
                Description = (string)r["snippet"],
                Url = (string)r["link"]
            });

            return results;
        }

        protected virtual string PrepareRequestUrl(string searchTerm)
        {
            return string.Format("?key={0}&cx={1}&q={2}&alt=json&num={3}", ApiKey, CtxKey, searchTerm, SearchConstants.SEARCH_COUNT);
        }
    }
}