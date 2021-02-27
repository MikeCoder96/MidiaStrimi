using CloudFlareUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace API_Core.Hosts.Websites
{
    class AltaDefinizione_Wrapper : AbstractStreamPage
    {
        private string actual_url = null;
        private string retrieveLinkMovies()
        {
            if (actual_url == null)
            {
                WebClient test = new WebClient();
                var extr = test.DownloadString("https://altadefinizione-nuovo.link");
                HtmlAgilityPack.HtmlDocument doctodown = new HtmlAgilityPack.HtmlDocument();
                doctodown.LoadHtml(extr);
                var NodesLink = doctodown.DocumentNode.SelectNodes("/html/body/div[1]/div/div/section[2]/div/div/div/div/div/section/div/div/div[2]/div/div/div[2]/div/h2/a");
                actual_url = NodesLink[0].Attributes.FirstOrDefault().Value;
            }
            return actual_url;
        }
        private string retrieveLinkSeries()
        {
            if (actual_url == null)
            {
                WebClient test = new WebClient();
                var extr = test.DownloadString("https://nuovoindirizzo.info/seriehd/");
                HtmlAgilityPack.HtmlDocument doctodown = new HtmlAgilityPack.HtmlDocument();
                doctodown.LoadHtml(extr);
                var NodesLink = doctodown.DocumentNode.SelectNodes("/html/body/div[1]/div/div/section[2]/div/div/div/div/div/section/div/div/div[2]/div/div/div[2]/div/h2/a");
                actual_url = NodesLink[0].Attributes.FirstOrDefault().Value;
            }
            return actual_url;
        }

        public override void retrieveTvStreamLinks(TvSerie serie)
        {
            return;
        }

        ~AltaDefinizione_Wrapper()
        {
            Console.WriteLine("Host is not required anymore");
        }

        public override List<Movie> retreiveTopMovies()
        {
            return null;
        }

        private string getSerieDescr(Uri link)
        {
            var handler = new ClearanceHandler();
            var client = new HttpClient(handler);
            try
            {
                var content = client.GetStringAsync(link).Result;
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(content);
                var nodeSelected = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"infoSerie\"]/div/div/div[2]/div/div[2]/p[2]");
                return nodeSelected.InnerText;
            }
            catch { }

            return "NO DESCRIPTION";
        }

        private string getMovieeDescr(Uri link)
        {
            var handler = new ClearanceHandler();
            var client = new HttpClient(handler);
            try
            {
                var content = client.GetStringAsync(link).Result;
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(content);
                var nodeSelected = htmlDoc.DocumentNode.SelectSingleNode("/html/body/section[3]/div/div/div[1]/div[7]/div[2]/p/text()");
                return nodeSelected.InnerText;
            }
            catch { }

            return "NO DESCRIPTION";
        }

        public override List<TvSerie> searchTvSeries(string tmp_title)
        {
            var target = new Uri(retrieveLinkSeries() + "?s=" + tmp_title);
            var handler = new ClearanceHandler();
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

                    //Weird way to delete IMDB text
                    string[] tmpTit = node.InnerText.Split();
                    tmpTit = tmpTit.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    tmpTit[tmpTit.Length - 1] = "";
                    tmpTit[tmpTit.Length - 2] = "";
                    tmpTit = tmpTit.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    string finalTitle = string.Join(" ", tmpTit);
                    //Dpn't be so rude with me boy..

                    string serieTitle = WebUtility.HtmlDecode(WebUtility.HtmlDecode(finalTitle));

                    tmp.Add(new TvSerie(serieTitle, getSerieDescr(serieLink), null, serieLink));
                }
                return tmp;
            }
            catch (Exception ex)
            { }

            return null;
        }

        public override List<Movie> searchMovie(string tmp_title)
        {
            var target = new Uri(retrieveLinkMovies() + "?s=" + tmp_title);
            var handler = new ClearanceHandler();
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
                    
                    var toUtf = node.SelectNodes("//h5[contains(@class, 'titleFilm')]");

                    string movieTitle = WebUtility.HtmlDecode(WebUtility.HtmlDecode(toUtf[indexTitle++].InnerText));

                    tmp.Add(new Movie(movieTitle, WebUtility.HtmlDecode(WebUtility.HtmlDecode(getMovieeDescr(movieLink))), null, movieLink));
                }
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

        public override void retrieveStreamLinks(Movie movie)
        {
            var target = movie.getMoviePageLink();
            var handler = new ClearanceHandler();
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
                    return;
                }
                //Travel To Player of HDPass
                var node = nodes[0].ChildNodes[1].Attributes[3].Value;
                target = new Uri(node);
                handler = new ClearanceHandler();
                client = new HttpClient(handler);
                content = client.GetStringAsync(target);
                htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(content.Result);

                //Get the hosts
                nodes = htmlDoc.DocumentNode.SelectNodes("/html/body/div[1]/div[2]/ul");
                var nds = nodes[0].ChildNodes;
                for (int i = 1; i < nds.Count; i += 2)
                {
                    var target_1 = new Uri(WebUtility.HtmlDecode(nds[i].ChildNodes[0].Attributes[0].Value).Replace("&alta=", "&play_chosen="));
                    var providerInt = int.Parse(getBetween(nds[i].ChildNodes[0].Attributes[0].Value, "host=", "&"));
                    var handler_1 = new ClearanceHandler();
                    var client_1 = new HttpClient(handler_1);

                    var content_1 = client.GetStringAsync(target_1);
                    var htmlDoc_1 = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc_1.LoadHtml(content_1.Result);
                    var nodo_1 = htmlDoc_1.DocumentNode.SelectSingleNode("//*[@id=\"main-player-wrapper\"]/iframe");
                    string FinalUrl = "";
                    string provider = htmlDoc_1.DocumentNode.SelectSingleNode("//*[@id=\"host-" + providerInt.ToString() + "\"]").FirstChild.InnerText;
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
            }
            catch (AggregateException ex) when (ex.InnerException is CloudFlareClearanceException)
            {
                Console.WriteLine(ex.InnerException.Message);
                return;
            }
        }
    }
}
