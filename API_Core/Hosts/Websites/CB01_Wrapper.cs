﻿using CloudProxySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

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

        private string cleanDescription(string toClean)
        {
            try
            {
                Regex to_check = new Regex(@"(?:\))(.*)(?:\s+)");
                var htmlCleaned = WebUtility.HtmlDecode(toClean);
                var result = to_check.Match(htmlCleaned).Groups[1].Value;
                return result;
            }
            catch
            {
                return "NO DESCRIPTION";
            }          
        }

        public override List<TvSerie> searchTvSeries(string tmp_title)
        {
            string title = retrieveLink() + "/serietv/?s=" + tmp_title;
            List<TvSerie> tvList = new List<TvSerie>();
            try
            {
                var target = new Uri(title);
                var handler = new ClearanceHandler("http://localhost:8191/")
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36",
                    MaxTimeout = 60000
                };
                var client = new HttpClient(handler);
                var content = client.GetStringAsync(target);
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(content.Result);
                var nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'post-')]");
                if (nodes == null)
                {
                    return null;
                }
                foreach (HtmlAgilityPack.HtmlNode node in nodes)
                {
                    string ImageLink = node.Descendants("img").FirstOrDefault().Attributes["src"].Value;
                    Uri MovieLink = new Uri(node.Descendants("a").FirstOrDefault().Attributes[0].Value);
                    string MovieTitle = node.Descendants("h3").FirstOrDefault().InnerText;
                    var DescriptionData = cleanDescription(node.ChildNodes[1].ChildNodes[3].InnerText);

                    tvList.Add(new TvSerie(WebUtility.HtmlDecode(MovieTitle), DescriptionData, 1, ImageLink, MovieLink));
                }
                foreach (var x in tvList)
                    retrieveTvStreamLinks(x);

                return tvList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                return null;
            }
        }

        public override void retrieveTvStreamLinks(TvSerie serie)
        {
            try
            {
                var target = serie.getSeriePageLink();
                var handler = new ClearanceHandler("http://localhost:8191/")
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36",
                    MaxTimeout = 60000
                };
                var client = new HttpClient(handler);
                var content = client.GetStringAsync(target);
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(content.Result);
                var node = htmlDoc.DocumentNode.SelectNodes("//*[@id=\"sequex-main-inner\"]/div[1]/article/div[1]/table[1]/tbody/tr/td/div/div[2]");
                if (node == null)
                    return;
                foreach (var parent in node)
                {
                    foreach (var toAnalyze in parent.ChildNodes)
                    {
                        if (toAnalyze.InnerHtml.ToLower().Contains("stayonline"))
                        {
                            string episode = WebUtility.HtmlDecode(toAnalyze.ChildNodes.FirstOrDefault().InnerText).Replace(" -", "");
                            Dictionary<string, string> links = new Dictionary<string, string>();
                            for (int i = 1; i < toAnalyze.ChildNodes.Count; i++)
                            {
                                if (!toAnalyze.ChildNodes[i].Name.ToLower().Contains("text"))
                                {
                                    //It need to be redone
                                    try
                                    {
                                        string provider = toAnalyze.ChildNodes[i].InnerText;
                                        string link = toAnalyze.ChildNodes[i].Attributes[0].Value;
                                        links.Add(provider, link);
                                    }
                                    catch
                                    { }
                                }
                            }
                            serie.addLink(episode, links);
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
            }
        }

        public override List<Movie> searchMovie(string tmp_title)
        {
            string title = retrieveLink() + "/page/1/?s=" + tmp_title;
            List<Movie> mvList = new List<Movie>();
            int lastPage = 1;
            try
            {
                var target = new Uri(title);
                var handler = new ClearanceHandler("http://localhost:8191/")
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36",
                    MaxTimeout = 60000
                };
                var client = new HttpClient(handler);
                var content = client.GetStringAsync(title);
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
                    client = new HttpClient(handler);
                    content = client.GetStringAsync(target);
                    htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.LoadHtml(content.Result);
                    nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'post-')]");
                    foreach (HtmlAgilityPack.HtmlNode node in nodes)
                    {
                        var tmp = node.SelectSingleNode("//*[contains(@class, 'rating-icons')]");
                        string ImageLink = node.Descendants("img").FirstOrDefault().Attributes["src"].Value;
                        Uri MovieLink = new Uri(node.Descendants("a").FirstOrDefault().Attributes[0].Value);
                        string MovieTitle = node.Descendants("h3").FirstOrDefault().InnerText;
                        var DescriptionData = node.Descendants("p").FirstOrDefault().InnerText;
                        //var points = node.SelectSingleNode(".//div[contains(@id, 'pd_rating_holder_')]");

                        mvList.Add(new Movie(WebUtility.HtmlDecode(MovieTitle), WebUtility.HtmlDecode(DescriptionData), 1, ImageLink, MovieLink));
                    }
                    int suc = i + 1;
                    title = title.Replace("/" + i + "/", "/" + suc + "/");
                }
                for (int i = 0; i < mvList.Count; i++)
                    if (!retrieveStreamLinks(mvList[i]))
                        mvList.Remove(mvList[i]);

                return mvList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                return null;
            }
        }

        public override List<Movie> retreiveTopMovies()
        {
            string title = retrieveLink();
            List<Movie> mvList = new List<Movie>();
            try
            {
                var target = new Uri(title);
                var handler = new ClearanceHandler("http://localhost:8191/")
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36",
                    MaxTimeout = 60000
                };
                var client = new HttpClient(handler);
                var content = client.GetStringAsync(target);
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(content.Result);
                var node = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'wa_chpcs_image_carousel')]");
                if (node == null)
                    return null;

                target = new Uri(title);
                client = new HttpClient(handler);
                content = client.GetStringAsync(target);
                htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(content.Result);
                var nodes = htmlDoc.DocumentNode.SelectNodes("//li[contains(@id, 'wa_chpcs_foo_content')]");
                foreach (HtmlAgilityPack.HtmlNode nod in nodes)
                {
                    var res = nod.FirstChild;
                    string ImageLink = res.Descendants("img").FirstOrDefault().Attributes["src"].Value;
                    Uri MovieLink = new Uri(res.Descendants("a").FirstOrDefault().Attributes[0].Value);
                    string DescriptionData = "";
                    mvList.Add(new Movie("", WebUtility.HtmlDecode(DescriptionData), 1, ImageLink, MovieLink));
                }
                for (int i = 0; i < mvList.Count; i++)
                    if (!retrieveStreamLinks(mvList[i]))
                        mvList.Remove(mvList[i]);

                return mvList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                return null;
            }
        }

        ~CB01_Wrapper()
        {
            Console.WriteLine("Host is not required anymore");
        }

        public override bool retrieveStreamLinks(Movie movie)
        {
            try
            {
                var target = movie.getMoviePageLink();
                var handler = new ClearanceHandler("http://localhost:8191/")
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36",
                    MaxTimeout = 60000
                };
                var client = new HttpClient(handler);
                var content = client.GetStringAsync(target);
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(content.Result);
                var nodes = htmlDoc.DocumentNode.SelectNodes("//table[contains(@class, 'tableinside') or contains(@class, 'cbtable')]");
                if (nodes == null)
                    return false;

                if (movie.movieTitle == "")
                {
                    var tmpNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/main/div[1]/div[3]/h1");
                    movie.movieTitle = WebUtility.HtmlDecode(tmpNode.InnerText);
                }
                if (movie.movieDescription == "")
                {
                    var tmpNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"sequex-main-inner\"]/div[1]/article/div[1]/p[2]");
                    movie.movieDescription = WebUtility.HtmlDecode(tmpNode.InnerText);
                }

                bool HD = false;
                foreach (HtmlAgilityPack.HtmlNode node in nodes)
                {
                    try
                    {
                        if (node.InnerText.ToLower().Contains("download:") && node.InnerText.ToLower().Contains("streaming:"))
                            continue;

                        else if (node.InnerText.ToLower().Contains("streaming hd:"))
                            HD = true;

                        else if (node.InnerText.ToLower().Contains("download:"))
                            break;

                        if (node.SelectSingleNode(".//a") == null)
                            continue;

                        string link = node.SelectSingleNode(".//a").Attributes["href"].Value;

                        string provider = "";
                        if(HD)
                            provider = "[HD] " + node.SelectSingleNode(".//a").InnerText;
                        else
                            provider = node.SelectSingleNode(".//a").InnerText;

                        if (link.StartsWith("https") || link.StartsWith("http"))
                            movie.addLink(provider, link);
                    }
                    catch {}
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                return false;
            }
        }
    }
}
