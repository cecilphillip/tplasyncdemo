using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy;
using WebSearch.API.WebSearch;
using Xunit;

namespace WebSearch.API.Tests
{
    public class BingSearchControllerTests
    {
        [Fact]
        public void Get()
        {
            var searchEngine = A.Fake<ISearchEngine>();
         //   A.CallTo(() => searchEngine.WebSearch(A<string>.Ignored)).Returns();
        }
    }
}
