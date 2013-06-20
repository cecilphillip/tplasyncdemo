using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebSearch.API.WebSearch;

namespace WebSearch.API.API
{
    public class CompositeSearchController : ApiController
    {
         protected ISearchEngine Engine { get; set; }

         public CompositeSearchController(ISearchEngine engine)
        {
            Engine = engine;
        }

        
        public Task<IEnumerable<SearchResult>> Get(string searchTerm)
        {
            return Engine.WebSearch(searchTerm);
        }
    }
}