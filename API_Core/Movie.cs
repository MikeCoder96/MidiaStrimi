using System;
using System.Collections.Generic;
using System.Text;

namespace API_Core
{
    public class Movie
    {
        private readonly string movieTitle;
        private readonly string movieDescription;
        private readonly string movieImage;
        private readonly Uri pageLink;
        private readonly List<string> streamLinks;

        public Movie(string s_movieTitle, string s_movieDescription, string s_movieImage, Uri s_pageLink)
        {
            this.movieTitle = s_movieTitle;
            this.movieDescription = s_movieDescription;
            this.pageLink = s_pageLink;
            this.movieImage = s_movieImage;
            this.streamLinks = new List<string>();
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

        public void addLink(string link) {
            this.streamLinks.Add(link);
        }

        public List<string> getStreams() {
            return this.streamLinks;
        }
    }
}
