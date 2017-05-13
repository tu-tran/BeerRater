﻿namespace BeerRater.Providers.Utils
{
    using System;
    using System.Net;
    using System.Text;

    using HtmlAgilityPack;

    using RestSharp;

    /// <summary>
    /// The web extensions.
    /// </summary>
    public static class WebExtensions
    {
        /// <summary>
        /// Gets the web document.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="referrer">The referrer.</param>
        /// <param name="isMobile">A value indicating whether to.</param>
        /// <returns>The web document.</returns>
        public static HtmlDocument GetDocument(this string url, string referrer = "", bool isMobile = true)
        {
            var htmlDoc = new HtmlDocument();
            var request = GetRequest(url, referrer, isMobile);
            using (var respStream = request.GetResponse().GetResponseStream())
            {
                if (respStream != null)
                    htmlDoc.Load(respStream, true);
            }

            return htmlDoc;
        }

        /// <summary>
        /// Gets the rest response.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="referrerUrl">The referrer URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="format">The format.</param>
        /// <param name="isMobile">if set to <c>true</c> [is mobile].</param>
        /// <returns>The response.</returns>
        public static string GetRestResponse(this string url, string referrerUrl = "", Method method = Method.GET, DataFormat format = DataFormat.Json, bool isMobile = true)
        {
            var client = new RestClient(url) { Encoding = Encoding.Default };
            var request = new RestRequest(".", method) { RequestFormat = format };
            request.AddHeader("Referer", referrerUrl);
            request.AddHeader("User-Agent", WebExtensions.GetUserAgent(isMobile));

            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Content;
            }

            return null;
        }

        /// <summary>
        /// Does URL exist.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="referrer">The referrer.</param>
        /// <returns>True if exists.</returns>
        public static bool UrlExists(this string url, string referrer = "")
        {
            var request = GetRequest(url, referrer, true);
            request.Method = "HEAD";
            try
            {
                using (request.GetResponse())
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the web request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="referrer">The referrer.</param>
        /// <param name="isMobile">A value indicating whether to.</param>
        /// <returns>The web request.</returns>
        public static HttpWebRequest GetRequest(this string url, string referrer = "", bool isMobile = true)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = GetUserAgent(isMobile);
            request.Referer = referrer;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            return request;
        }

        /// <summary>
        /// Decodes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The decoded text.</returns>
        public static string Decode(this string text)
        {
            return WebUtility.HtmlDecode(text ?? "");
        }

        /// <summary>
        /// Decodes and trims the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The decoded and trimmed text.</returns>
        public static string TrimDecoded(this string text)
        {
            return text.Decode().Trim();
        }

        /// <summary>
        /// Gets the user agent.
        /// </summary>
        /// <param name="isMobile">A value indicating whether is for mobile.</param>
        /// <returns>The user agent string.</returns>
        public static string GetUserAgent(bool isMobile = true)
        {
            return isMobile
                       ? "Mozilla/5.0 (Linux; U; Android 4.2; en-us; SonyC6903 Build/14.1.G.1.518) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30"
                       : "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; AS; rv:11.0) like Gecko";
        }
    }
}