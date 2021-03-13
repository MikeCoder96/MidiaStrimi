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
        private string movieTitle { get; set; }
        private string movieDescription { get; set; }
        private string movieType { get; set; }
        private string movieDuration { get; set; }
        private string movieImage { get; set; }
        private int movieProvider { get; set; }
        private string movieScore { get; set; }
        private Uri pageLink { get; set; }
        private List<stream> streamLinks { get; set; }

        public Movie(string s_movieTitle, string s_movieDescription, int i_movieProvider, string s_movieImage, Uri s_pageLink, string s_movieType = "", string s_movieDuration = "", string s_movieScore = "N/A")
        {
            this.movieTitle = s_movieTitle;
            this.movieDescription = s_movieDescription;
            this.pageLink = s_pageLink;
            this.movieProvider = i_movieProvider;
            this.movieImage = s_movieImage;
            this.movieScore = s_movieScore;
            this.movieDuration = s_movieDuration;
            this.movieType = s_movieType;
            this.streamLinks = new List<stream> ();
        }

        #region Getter
        public List<stream> getStreams()
        {
            return this.streamLinks;
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

        public string getMovieDuration()
        {
            return this.movieDuration;
        }

        public string getMovieType()
        {
           return this.movieType;
        }
        #endregion

        #region Setter
        public void addLink(string provider, string link) {
            this.streamLinks.Add(new stream(provider, link));
        }
        
        public void setMoviePoints(string x)
        {
            this.movieScore = x;
        }

        public void setMovieDuration(string x)
        {
            this.movieDuration = x;
        }

        public void setMovieType(string x)
        {
            this.movieType = x;
        }
        #endregion

    }
}
