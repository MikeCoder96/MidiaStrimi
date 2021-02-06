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
        private string retrieveLink()
        {
            if (actual_url == null)
            {
                WebClient test = new WebClient();
                var extr = test.DownloadString("https://altadefinizione-nuovo.link");
                HtmlAgilityPack.HtmlDocument doctodown = new HtmlAgilityPack.HtmlDocument();
                doctodown.LoadHtml(extr);
                var NodesLink = doctodown.DocumentNode.SelectNodes("/html/body/div[1]/div/div/section[2]/div/div/div/div/div/section/div/div/div[2]/div/div/div[2]/div/h2/a");
                actual_url = NodesLink[0].InnerText;
            }
            return actual_url;
        }

        public override List<Movie> retreiveTopMovies()
        {
            return null;
        }

        public override List<Movie> searchMovie(string tmp_title)
        {
            var target = new Uri("https://" + retrieveLink() + "/?s=" + tmp_title);
            var handler = new ClearanceHandler();
            var client = new HttpClient(handler);
            List<Movie> tmp = new List<Movie>();

            try
            {
                var content = client.GetStringAsync(target).Result;
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(content);
                var nodes = htmlDoc.DocumentNode.SelectNodes("//section[contains(@id, 'lastUpdate')]");
                if (nodes == null)
                {
                    return null;
                }
                foreach (HtmlAgilityPack.HtmlNode node in nodes[0].ChildNodes[1].ChildNodes)
                {
                    string imageLink = node.Descendants("img").FirstOrDefault().Attributes["src"].Value;
                    Uri movieLink = new Uri(node.Descendants("a").FirstOrDefault().Attributes[0].Value);

                    //Weird way to delete IMDB text
                    string[] tmpTit = node.InnerText.Split();
                    tmpTit = tmpTit.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    tmpTit[tmpTit.Length - 1] = "";
                    tmpTit[tmpTit.Length - 2] = "";
                    tmpTit = tmpTit.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    string finalTitle = string.Join(" ", tmpTit);
                    //Dpn't be so rude with me boy..

                    string movieTitle = WebUtility.HtmlDecode(WebUtility.HtmlDecode(finalTitle));

                    tmp.Add(new Movie(movieTitle, "NO DESCRIPTION", null, movieLink));
                }
                return tmp;
            }
            catch { }

            return null;
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
                    var handler_1 = new ClearanceHandler();
                    var client_1 = new HttpClient(handler_1);

                    var content_1 = client.GetStringAsync(target_1);
                    var htmlDoc_1 = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc_1.LoadHtml(content_1.Result);
                    var nodo_1 = htmlDoc_1.DocumentNode.SelectSingleNode("/html/body/div[3]/iframe");
                    byte[] data = System.Convert.FromBase64String(nodo_1.Attributes[1].Value);
                    string FinalUrl = System.Text.ASCIIEncoding.ASCII.GetString(data);
                    movie.addLink(null, FinalUrl);
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
