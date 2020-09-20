using System;
using System.Linq;
using BeerRater.Data;
using BeerRater.Providers.Utils;
using BeerRater.Utils;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace BeerRater.Providers.Ratings
{
    /// <summary>
    ///     The web crawler.
    /// </summary>
    public class RateBeerProvider : IRatingProvider
    {
        /// <summary>
        ///     Queries the specified beer release name.
        /// </summary>
        /// <param name="beerInfo">The beer information.</param>
        public void Query(BeerInfo beerInfo)
        {
            GetBeerRateMetadata(beerInfo);
        }

        /// <summary>
        ///     Gets the beer result.
        /// </summary>
        /// <param name="beerInfo">The beer information.</param>
        /// <returns>True on success; otherwise, false.</returns>
        public static bool GetBeerRateMetadata(BeerInfo beerInfo)
        {
            var beerUrl = string.Empty;
            var apiUrl = "https://beta.ratebeer.com/v1/api/graphql/";
            var referrer = "https://www.ratebeer.com";
            var firstQuery = apiUrl.GetRestResponseContent(referrer, Method.OPTIONS);
            var response = apiUrl.GetRestResponse(referrer, Method.POST, DataFormat.Json, true,
                new object[]
                {
                    new
                    {
                        operationName = "beerSearch",
                        variables = new
                        {
                            query = beerInfo.Name,
                            first = 1
                        },
                        query =
                            "query beerSearch($query: String, $first: Int) {\n  results: beerSearch(query: $query, first: $first) {\n    items {\n      beer {\n        id\n        name\n        __typename\n      }\n      __typename\n    }\n    __typename\n  }\n}\n"
                    }
                });

            var metaData = JArray.Parse(response.Content).FirstOrDefault()?["data"];
            var metaResults = metaData?["results"];
            var metaItems = metaResults?["items"] as JArray;
            var metaItem = metaItems?.FirstOrDefault();
            var metaBeer = metaItem?["beer"];
            if (metaBeer == null) return false;

            var beerName = GetStrValue(metaBeer["name"]);
            var idStr = GetStrValue(metaBeer["id"]);
            var ceoName = beerName.URLFriendly();
            if (!string.IsNullOrWhiteSpace(idStr) && !string.IsNullOrWhiteSpace(ceoName))
            {
                beerInfo.Name = beerName;
                beerInfo.ReviewUrl = $"https://www.ratebeer.com/beer/{ceoName}/{idStr}/";
                var rateResponse = apiUrl.GetRestResponse(referrer, Method.POST, DataFormat.Json, true,
                    new object[]
                    {
                        new
                        {
                            operationName = "beer",
                            variables = new
                            {
                                beerId = idStr
                            },
                            query =
                                "query beer($beerId: ID!) {\n  info: beer(id: $beerId) {\n    id\n    name\n    description\n    style {\n      id\n      name\n      glasses {\n        id\n        name\n        __typename\n      }\n      __typename\n    }\n    styleScore\n    overallScore\n    averageRating\n    realAverage\n    abv\n    ibu\n    calories\n    brewer {\n      id\n      name\n      city\n      state {\n        id\n        name\n        __typename\n      }\n      country {\n        id\n        name\n        __typename\n      }\n      __typename\n    }\n    contractBrewer {\n      id\n      name\n      city\n      state {\n        id\n        name\n        __typename\n      }\n      country {\n        id\n        name\n        __typename\n      }\n      __typename\n    }\n    ratingCount\n    isRetired\n    isVerified\n    isUnrateable\n    seasonal\n    labels\n    availability {\n      bottle\n      tap\n      distribution\n      __typename\n    }\n    __typename\n  }\n}\n"
                        }
                    });

                var beerData = JArray.Parse(rateResponse.Content).FirstOrDefault()?["data"];
                var beerInfoNode = beerData?["info"];
                beerInfo.Overall = GetDoubleValue(beerInfoNode?["averageRating"]);
                beerInfo.WeightedAverage = GetDoubleValue(beerInfoNode?["realAverage"]);
                beerInfo.ABV = GetDoubleValue(beerInfoNode?["abv"]);
                beerInfo.Calories = GetDoubleValue(beerInfoNode?["calories"]);
                beerInfo.Ratings = GetIntValue(beerInfoNode?["ratingCount"]);

                var beerStyle = beerInfoNode?["style"];
                beerInfo.Style = GetStrValue(beerStyle?["name"]);
            }

            return true;
        }

        /// <summary>
        ///     Gets the string value.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        private static string GetStrValue(JToken token)
        {
            var val = token as JValue;
            if (val != null && val.Value != null) return val.Value.ToString();

            return string.Empty;
        }

        /// <summary>
        ///     Gets the string value.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        private static double GetDoubleValue(JToken token)
        {
            var val = token as JValue;
            if (val != null && val.Value is double) return (double) val.Value;

            var result = GetStrValue(token).ToDouble();
            return result ?? default;
        }

        /// <summary>
        ///     Gets the string value.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        private static int GetIntValue(JToken token)
        {
            var val = token as JValue;
            if (val != null && val.Value is int) return (int) val.Value;

            return Convert.ToInt32(GetDoubleValue(token));
        }
    }
}