﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using WebSearch.API.WebSearch;

namespace WebSearch.API.API
{
    public class BingSearchController : ApiController
    {
        protected ISearchEngine Engine { get; set; }

        public BingSearchController(ISearchEngine engine)
        {
            Engine = engine;
        }
        
        public Task<IEnumerable<SearchResult>> Get(string searchTerm)
        {
            return Engine.WebSearch(searchTerm);
        }
    }
}