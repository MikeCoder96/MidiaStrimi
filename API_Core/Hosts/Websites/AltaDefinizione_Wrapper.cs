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
                actual_url =  NodesLink[0].InnerText;
            }
            return actual_url;
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

                    tmp.Add(new Movie(movieTitle, "NO DESCRIPTION", movieLink));
                }
                return tmp;
            }
            catch { }

            return null;
        }
    }
}
