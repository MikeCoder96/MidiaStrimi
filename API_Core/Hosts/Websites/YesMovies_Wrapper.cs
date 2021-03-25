using CloudProxySharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace API_Core.Hosts.Websites
{
    class YesMovies_Wrapper : AbstractStreamPage
    {
        private string actual_url = "https://yesmovies.ag/";
        private string retrieveLink()
        {
            //TODO
            return actual_url;
        }

        ~YesMovies_Wrapper()
        {
            Console.WriteLine("Host is not required anymore");
        }
        public override List<Movie> retreiveTopMovies()
        {
            return null;
        }

        public override void retrieveTvStreamLinks(TvSerie serie)
        {
            //TODO
        }

        public override List<TvSerie> searchTvSeries(string tmp_title)
        {
            return null;
        }

        public override List<Movie> searchMovie(string tmp_title)
        {
            var target = new Uri(retrieveLink() + "searching/" + tmp_title + ".html");
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
                
                return tmp;
            }
            catch (Exception ex)
            {  }

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
