using CloudFlareUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace API_Core.Hosts.CB01
{
    class CB01_Wrapper : AbstractStreamPage
    {
        public override string retrieveLink()
        {
            WebClient test = new WebClient();
            var extr = test.DownloadString("https://www.cb01.community/");
            HtmlAgilityPack.HtmlDocument doctodown = new HtmlAgilityPack.HtmlDocument();
            doctodown.LoadHtml(extr);
            return doctodown.DocumentNode.SelectNodes("/html/body/div/h2/a/b")[0].InnerText;
        }
        public override List<Movie> searchMovie(string tmp_title)
        {
            string title = retrieveLink() + "/page/1/?s=" + tmp_title;
            List<Movie> mvList = new List<Movie>();
            int maxPages = 5;
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
                /*if (!CB01.isFirst)
                {
                    var numNodes = htmlDoc.DocumentNode.SelectNodes("//ul[contains(@class, 'pagination')]/li[contains(@class, 'page-item')]");
                    if (numNodes != null)
                    {
                        Form1.Self.Working = true;
                        string number = numNodes[numNodes.Count() - 2].InnerText;
                        if (number.Contains('.'))
                            lastPage = Int32.Parse(number.Replace(".", ""));
                        else lastPage = Int32.Parse(numNodes[numNodes.Count() - 2].InnerText);
                    }
                }
                else
                {
                    lastPage = 5;
                    CB01.isFirst = false;
                }*/
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
                        string MovieLink = node.Descendants("a").FirstOrDefault().Attributes[0].Value;
                        string MovieTitle = node.Descendants("h3").FirstOrDefault().InnerText;
                        string DescriptionData = node.Descendants("p").FirstOrDefault().InnerText;

                        mvList.Add(new Movie(WebUtility.HtmlDecode(MovieTitle), WebUtility.HtmlDecode(DescriptionData)));
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
            return null;
        }
    }
}
