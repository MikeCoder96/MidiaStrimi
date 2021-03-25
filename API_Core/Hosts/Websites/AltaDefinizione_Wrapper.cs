using CloudProxySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace API_Core.Hosts.Websites
{
    class AltaDefinizione_Wrapper : AbstractStreamPage
    {
        private string actual_url_movie = null;
        private string actual_url_serie = null;
        private string retrieveLinkMovies()
        {
            if (actual_url_movie == null)
            {
                WebClient test = new WebClient();
                var extr = test.DownloadString("https://altadefinizione-nuovo.link");
                HtmlAgilityPack.HtmlDocument doctodown = new HtmlAgilityPack.HtmlDocument();
                doctodown.LoadHtml(extr);
                var NodesLink = doctodown.DocumentNode.SelectNodes("/html/body/div[1]/div/div/section[2]/div/div/div/div/div/section/div/div/div[2]/div/div/div[2]/div/h2/a");
                actual_url_movie = NodesLink[0].Attributes.FirstOrDefault().Value;
            }
            return actual_url_movie;
        }
        private string retrieveLinkSeries()
        {
            if (actual_url_serie == null)
            {
                WebClient test = new WebClient();
                var extr = test.DownloadString("https://nuovoindirizzo.info/seriehd/");
                HtmlAgilityPack.HtmlDocument doctodown = new HtmlAgilityPack.HtmlDocument();
                doctodown.LoadHtml(extr);
                var NodesLink = doctodown.DocumentNode.SelectNodes("/html/body/div[1]/div/div/section[2]/div/div/div/div/div/section/div/div/div[2]/div/div/div[2]/div/h2/a");
                actual_url_serie = NodesLink[0].Attributes.FirstOrDefault().Value;
            }
            return actual_url_serie;
        }

        public override void retrieveTvStreamLinks(TvSerie serie)
        {
            var handler = new ClearanceHandler("http://localhost:8191/")
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36",
                MaxTimeout = 60000
            };
            var client = new HttpClient(handler);
            try
            {
                var content = client.GetStringAsync(serie.getSeriePageLink()).Result;
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(content);
                var nodeDescription = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"infoSerie\"]/div/div/div[2]/div/div[2]/p[2]");
                serie.SerieDescription = WebUtility.HtmlDecode(nodeDescription.InnerText);
                var nodeSelected = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"iframeVid\"]");
                Dictionary<string, string> tmp = new Dictionary<string, string>();
                tmp.Add("MainPanel", nodeSelected.Attributes[3].Value);
                serie.addLink("All Episodes", tmp);
                return;
            }
            catch { }
        }

        ~AltaDefinizione_Wrapper()
        {
            Console.WriteLine("Host is not required anymore");
        }

        public override List<Movie> retreiveTopMovies()
        {
            //html/body/section[3]/div/div[2]/div[1]/div[2]/div/div[1]
            string title = retrieveLinkMovies();
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
                var node = htmlDoc.DocumentNode.SelectSingleNode("//*[contains(@class,'slides')]");
                if (node == null)
                    return null;
                var nodes = node.ChildNodes;
                List<Movie> tmp = new List<Movie>();
                foreach (HtmlAgilityPack.HtmlNode nod in nodes)
                {
                    if (nod.Name.Contains("#text"))
                        continue;
                    string imageLink = nod.Descendants("img").FirstOrDefault().Attributes["src"].Value;
                    Uri movieLink = new Uri(nod.Descendants("a").FirstOrDefault().Attributes[0].Value);

                    var toUtf = nod.Descendants("img").FirstOrDefault().Attributes["alt"].Value;
                    string movieTitle = WebUtility.HtmlDecode(WebUtility.HtmlDecode(toUtf).Replace("streaming", ""));

                    tmp.Add(new Movie(movieTitle, "", 2, imageLink, movieLink));
                }
                for (int i = 0; i < tmp.Count; i++)
                    if (!retrieveStreamLinks(tmp[i]))
                        tmp.Remove(tmp[i]);

                return tmp;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                return null;
            }
        }

        public override List<TvSerie> searchTvSeries(string tmp_title)
        {
            var target = new Uri(retrieveLinkSeries() + "?s=" + tmp_title);
            var handler = new ClearanceHandler("http://localhost:8191/")
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36",
                MaxTimeout = 60000
            };
            var client = new HttpClient(handler);
            List<TvSerie> tmp = new List<TvSerie>();

            try
            {
                var content = client.GetStringAsync(target).Result;
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(content);
                var nodeSelected = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"library\"]/div/div/div[2]/div/div[1]/div[2]");
                if (nodeSelected == null)
                    return null;

                foreach (HtmlAgilityPack.HtmlNode node in nodeSelected.SelectNodes("//div[contains(@class, 'col-xl-3 col-lg-3 col-md-3 col-sm-6 col-6')]"))
                {
                    string imageLink = node.Descendants("img").FirstOrDefault().Attributes["src"].Value;
                    Uri serieLink = new Uri(node.Descendants("a").FirstOrDefault().Attributes[0].Value);

                    var toUTF = node.SelectSingleNode(".//div[contains(@class, 'infoSeries')]").ChildNodes[1].InnerText;
                    var points = node.SelectSingleNode(".//span[contains(@class, 'imdb')]").FirstChild.InnerText;
                    string serieTitle = WebUtility.HtmlDecode(toUTF);

                    tmp.Add(new TvSerie(serieTitle, "", 2, imageLink, serieLink, points));
                }
                foreach (var x in tmp)
                    retrieveTvStreamLinks(x);

                return tmp;
            }
            catch (Exception ex)
            { }

            return null;
        }

        public override List<Movie> searchMovie(string tmp_title)
        {
            var target = new Uri(retrieveLinkMovies() + "?s=" + tmp_title);
            var handler = new ClearanceHandler("http://localhost:8191/")
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36",
                MaxTimeout = 60000
            };
            var client = new HttpClient(handler);
            List<Movie> tmp = new List<Movie>();

            try
            {
                var content = client.GetStringAsync(target).Result;
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(content);
                var nodeSelected = htmlDoc.DocumentNode.SelectSingleNode("//section[contains(@id, 'lastUpdate')]");
                if (nodeSelected == null)
                    return null;

                int indexTitle = 0;
                foreach (HtmlAgilityPack.HtmlNode node in nodeSelected.SelectNodes("//div[contains(@class, 'col-lg-3 col-md-4')]"))
                {
                    string imageLink = node.Descendants("img").FirstOrDefault().Attributes["src"].Value;
                    Uri movieLink = new Uri(node.Descendants("a").FirstOrDefault().Attributes[0].Value);
                    
                    var toUtf = node.SelectSingleNode(".//h5[contains(@class, 'titleFilm')]");
                    var points = WebUtility.HtmlDecode(node.SelectSingleNode(".//div[contains(@class, 'imdb-rate')]").InnerText).Trim().Replace("IMDB:", "");

                    string movieTitle = WebUtility.HtmlDecode(toUtf.InnerText);

                    tmp.Add(new Movie(movieTitle, "", 2, imageLink, movieLink));
                }
                for (int i = 0; i < tmp.Count; i++)
                    if (!retrieveStreamLinks(tmp[i]))
                        tmp.Remove(tmp[i]);

                return tmp;
            }
            catch { }

            return null;
        }

        public string getBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }

            return "";
        }

        public override bool retrieveStreamLinks(Movie movie)
        {
            var target = movie.getMoviePageLink();
            var handler = new ClearanceHandler("http://localhost:8191/")
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36",
                MaxTimeout = 60000
            };
            var client = new HttpClient(handler);

            try
            {
                //Load Main Page
                var content = client.GetStringAsync(target);
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(content.Result);
                var nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'playerTemp')]");
                if (nodes == null)
                {
                    Console.WriteLine("Something goes terrible wrong...");
                    return false;
                }

                var nodeDescription = htmlDoc.DocumentNode.SelectSingleNode("/html/body/section[3]/div/div/div[1]/div[7]/div[2]/p/text()");
                movie.movieDescription = WebUtility.HtmlDecode(nodeDescription.InnerText);

                //Travel To Player of HDPass
                var nodeA = nodes[0].ChildNodes[1];
                if (nodeA.InnerText.Contains("non trovato"))
                    return false;

                var node = nodeA.Attributes[3].Value;
                target = new Uri(node);
                client = new HttpClient(handler);
                content = client.GetStringAsync(target);
                htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(content.Result);

                //Get the hosts
                //nodes = htmlDoc.DocumentNode.SelectNodes("/html/body/div[1]/div[2]/ul");
                nodes = htmlDoc.DocumentNode.SelectNodes("//*[contains(@id, 'host-')]");
                foreach (var host in nodes)
                {
                    var target_1 = new Uri(WebUtility.HtmlDecode(host.ChildNodes.FirstOrDefault().Attributes["href"].Value).Replace("&alta=", "&play_chosen="));
                    var client_1 = new HttpClient(handler);

                    var content_1 = client.GetStringAsync(target_1);
                    var htmlDoc_1 = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc_1.LoadHtml(content_1.Result);
                    var nodo_1 = htmlDoc_1.DocumentNode.SelectSingleNode("//*[@id=\"main-player-wrapper\"]/iframe");
                    string FinalUrl = "";
                    string provider = host.InnerText;
                    try
                    {
                        byte[] data = System.Convert.FromBase64String(nodo_1.Attributes[1].Value);
                        FinalUrl = System.Text.ASCIIEncoding.ASCII.GetString(data);
                    }
                    catch
                    {
                        FinalUrl = nodo_1.Attributes[1].Value;
                    }

                    movie.addLink(provider, FinalUrl);
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
