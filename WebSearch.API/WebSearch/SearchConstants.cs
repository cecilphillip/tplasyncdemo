using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSearch.API.WebSearch
{
    public static class SearchConstants
    {
        //Google CSE only allows a max of 10 results per query
        public const int SEARCH_COUNT = 10;
    }
}