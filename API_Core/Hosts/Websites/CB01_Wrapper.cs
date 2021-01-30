﻿using CloudFlareUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace API_Core.Hosts.Websites
{
    class CB01_Wrapper : AbstractStreamPage
    {
        private string actual_url = null;
        private string retrieveLink()
        {
            if (actual_url == null) {
                try
                {
                    IWebProxy defWebProxy = WebRequest.DefaultWebProxy;
                    defWebProxy.Credentials = CredentialCache.DefaultCredentials;

                    WebClient test = new WebClient { Proxy = defWebProxy };
                    var extr = test.DownloadString("https://www.cb01.community/");
                    HtmlAgilityPack.HtmlDocument doctodown = new HtmlAgilityPack.HtmlDocument();
                    doctodown.LoadHtml(extr);
                    actual_url = doctodown.DocumentNode.SelectNodes("/html/body/div/h2/a/b")[0].InnerText;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
            }
            return actual_url;
        }
        public override List<Movie> searchMovie(string tmp_title)
        {
            string title = retrieveLink() + "/page/1/?s=" + tmp_title;
            List<Movie> mvList = new List<Movie>();
            int lastPage = 1;
            try
            {
                var target = new Uri(title);
                var handler = new ClearanceHandler();
                var client = new HttpClient(handler);
                var content = client.GetStringAsync(target);
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(content.Result);
                var nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'post-')]");
                if (nodes == null)
                {
                    return null;
                }
               for (int i = 1; i <= lastPage; i++)
                {
                    target = new Uri(title);
                    handler = new ClearanceHandler();
                    client = new HttpClient(handler);
                    content = client.GetStringAsync(target);
                    htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.LoadHtml(content.Result);
                    nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'post-')]");
                    foreach (HtmlAgilityPack.HtmlNode node in nodes)
                    {
                        string ImageLink = node.Descendants("img").FirstOrDefault().Attributes["src"].Value;
                        Uri MovieLink = new Uri(node.Descendants("a").FirstOrDefault().Attributes[0].Value);
                        string MovieTitle = node.Descendants("h3").FirstOrDefault().InnerText;
                        string DescriptionData = node.Descendants("p").FirstOrDefault().InnerText;

                        mvList.Add(new Movie(WebUtility.HtmlDecode(MovieTitle), WebUtility.HtmlDecode(DescriptionData), ImageLink, MovieLink));
                    }
                    int suc = i + 1;
                    title = title.Replace("/" + i + "/", "/" + suc + "/");
                }
                return mvList;
            }
            catch (AggregateException ex) when (ex.InnerException is CloudFlareClearanceException)
            {
                Console.WriteLine(ex.InnerException.Message);
                return null;
            }
        }
        public override void retrieveStreamLinks(Movie movie)
        {
            try
            {
                var target = movie.getMoviePageLink();
                var handler = new ClearanceHandler();
                var client = new HttpClient(handler);
                var content = client.GetStringAsync(target);
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(content.Result);
                var nodes = htmlDoc.DocumentNode.SelectNodes("//table[contains(@class, 'tableinside')]");
                if (nodes == null)
                {
                    return;
                }

                target = movie.getMoviePageLink();
                handler = new ClearanceHandler();
                client = new HttpClient(handler);
                content = client.GetStringAsync(target);
                htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(content.Result);
                nodes = htmlDoc.DocumentNode.SelectNodes("//table[contains(@class, 'tableinside')]");
                foreach (HtmlAgilityPack.HtmlNode node in nodes)
                {
                    try
                    {
                        if (node.SelectSingleNode(".//a") == null)
                            continue;

                        string link = node.SelectSingleNode(".//a").Attributes["href"].Value;
                        if (link.StartsWith("https") || link.StartsWith("http"))
                            movie.addLink(link);    
                    }
                    catch { }
                }
                
            }
            catch (AggregateException ex) when (ex.InnerException is CloudFlareClearanceException)
            {
                Console.WriteLine(ex.InnerException.Message);
            }
        }
    }
}
