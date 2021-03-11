using System;
using System.Collections.Generic;
using System.Text;

namespace API_Core
{
    public class stream
    {
        public string provider { get; set; }
        public string link { get; set; }

        public stream(string provider, string link)
        {
            this.provider = provider;
            this.link = link;
        }
    }

    public class Movie
    {
        public string movieTitle { get; set; }
        public string movieDescription { get; set; }
        public string movieImage { get; set; }
        public int movieProvider { get; set; }
        public string movieScore { get; set; }
        public Uri pageLink { get; set; }
        public List<stream> streamLinks { get; set; }

        public Movie(string s_movieTitle, string s_movieDescription, int i_movieProvider, string s_movieImage, Uri s_pageLink, string s_movieScore = "N/A")
        {
            this.movieTitle = s_movieTitle;
            this.movieDescription = s_movieDescription;
            this.pageLink = s_pageLink;
            this.movieProvider = i_movieProvider;
            this.movieImage = s_movieImage;
            this.movieScore = s_movieScore;
            this.streamLinks = new List<stream> ();
        }

        public string getMovieTitle()
        {
            return this.movieTitle;
        }

        public string getMovieImage()
        {
            return this.movieImage;
        }

        public string getMovieDesc()
        {
            return this.movieDescription;
        }

        public Uri getMoviePageLink()
        {
            return this.pageLink;
        }

        public void addLink(string provider, string link) {
            this.streamLinks.Add(new stream(provider, link));
        }

        public List<stream> getStreams() {
            return this.streamLinks;
        }
    }
}
