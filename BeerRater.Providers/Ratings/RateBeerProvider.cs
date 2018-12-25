using System.Linq;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace BeerRater.Providers.Ratings
{
    using System.Text.RegularExpressions;

    using BeerRater.Utils;

    using Data;

    using Utils;

    /// <summary>
    /// The web crawler.
    /// </summary>
    public class RateBeerProvider : IRatingProvider
    {
        /// <summary>
        /// Queries the specified beer release name.
        /// </summary>
        /// <param name="beerInfo">The beer information.</param>
        public void Query(BeerInfo beerInfo)
        {
            var documentObject = GetBeerResult(beerInfo.Name);
        }

        /// <summary>
        /// Gets the beer result.
        /// </summary>
        /// <param name="beerName">Name of the beer.</param>
        /// <returns>The response string.</returns>
        public static string GetBeerResult(string beerName)
        {
            var queryUrl = $"https://beta.ratebeer.com/v1/api/graphql";
            var referrer = "https://www.ratebeer.com";
            var responseString = queryUrl.GetRestResponse(referrer, Method.POST, DataFormat.Json, true,
                new object[]
                {
                    new
                    {
                        operationName = "beerSearch",
                        query = @"query beerSearch($query: String, $order: SearchOrder, $first: Int, $after: ID) {
                        results: beerSearch(query: $query, order: $order, first: $first, after: $after) {
                        totalCount
                        last
                        items {
                        beer {
                        id
                        name
                        overallScore
                        ratingCount
                        __typename
                    }
                    review {
                    id
                    score
                    __typename
                    }
                    __typename
                    }
                    __typename
                    }
                    }",
                        variables = new {first = 1, order = "MATCH", query = beerName}
                    }
                });

            return responseString;
        }
    }
}